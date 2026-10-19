using Astropiano.ReflectionTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Astropiano.TotalSerializer.Configuration;

namespace Astropiano.TotalSerializer
{
    public class JsonWriter : JsonReadWriteBase<WriteState>
    {
        private readonly StreamWriter _streamWriter;
        private readonly StringBuilder _sb = new StringBuilder();
        //private readonly StringBuilder _finalSb = new StringBuilder();
        //TODO: compare performance with writing to a StringBuilder. Maybe using this when writing to string makes it faster

        private int _level;
        private Stack<LevelType> _levelTypes = new Stack<LevelType>();
        private bool _separatorRequired;
        
        private int _finalLevel;
        private Stack<LevelType> _finalLevelTypes = new Stack<LevelType>();
        private bool _finalSeparatorRequired;
        private WriteState _finalState;
        
        private Config _config;

        public JsonWriter(Config config)
        {
            _config = config;
            SetState(WriteState.OutsideObj);
        }
        
        public JsonWriter(Config config, StreamWriter streamWriter)
        {
            _config = config;
            _streamWriter = streamWriter;
            SetState(WriteState.OutsideObj);
        }

        public void Flush()
        {
            _streamWriter.Write(_sb.ToString());
            
            //_finalSb.Append(_sb);
            _finalLevel = _level;
            _finalLevelTypes = _levelTypes;
            _finalSeparatorRequired = _separatorRequired;
            _finalState = GetState();
            _sb.Clear();
        }
        
        public void RevertUnFlushed()
        {
            _sb.Clear();
            _level = _finalLevel;
            _levelTypes = _finalLevelTypes;
            _separatorRequired = _finalSeparatorRequired;
            SetState(_finalState);
        }
        
        

        public void StartObject()
        {
            AllowState(WriteState.OutsideObj, WriteState.WritingPropertyContent, WriteState.WritingArray);
            _sb.Append("{");
            SetState(WriteState.WritingObject);
            _level++;
            _levelTypes.Push(LevelType.Object);
            _separatorRequired = false;
        }

        public void WriteTypeInfo(Type type)
        {
            if (_config.IsIgnoredType(type))
            {
                throw new Exception("Currently writing a type that should get written.");
            }

            if (type.IsInterface)
            {
                throw new Exception("Writing interface type is not allowed, each value must" +
                                    " have a concrete type at the end.");
            }
            AllowState(WriteState.WritingObject);
            SetState(WriteState.WritingTypeInfo);
            _sb.Append("\"$type\":\"");
            TypeNameWriter.AppendTypeNameForJson(_sb, type);
            _sb.Append('\"');
            SetState(WriteState.WritingObject);
            _separatorRequired = true;
        }

        public void EndObject()
        {
            AllowState(WriteState.WritingObject);
            _sb.Append('}');
            _level--;
            _levelTypes.Pop();
            _separatorRequired = true;
            if (_level <= 0)
            {
                SetState(WriteState.OutsideObj);
            }
            else
            {
                SetStateBackToObjectOrArray();
            }
        }
        
        public void StartWriteProperty(string propertyName)
        {
            AllowState(WriteState.WritingObject);
            _sb.Append('\"');
            SetState(WriteState.WritingPropertyName);
            _sb.Append(propertyName);
            _sb.Append('\"');
            _sb.Append(':');
            SetState(WriteState.WritingPropertyContent);
        }

        public void EndWriteProperty()
        {
            AllowState(WriteState.WritingPropertyContent, WriteState.WritingObject);
            SetState(WriteState.WritingObject);
            _separatorRequired = true;
        }

        public void WritePrimitive(Type type, object value)
        {
            AllowState(WriteState.WritingPropertyContent, WriteState.WritingArray);
            string str = TypeWriter.ToString(type, value);
            _sb.Append(str);
            _separatorRequired = true;
        }
        
        public void WritePseudoPrimitive(Type type, object value)
        {
            AllowState(WriteState.WritingPropertyContent, WriteState.WritingArray);
            _sb.Append('{');
            _sb.Append("\"$type\":\"");
            TypeNameWriter.AppendTypeNameForJson(_sb, type);
            _sb.Append('\"');
            _sb.Append(',');
            string str = TypeWriter.ToString(type, value);
            _sb.Append("\"$value\":");
            _sb.Append(str);
            _sb.Append('}');
            _separatorRequired = true;
        }

      
        /*
        public string GetJson()
        {
            Flush();
            AllowState(WriteState.OutsideObj);
            return _finalSb.ToString();
        }*/

        public void WriteSeparatorIfRequired()
        {
            AllowState(WriteState.WritingObject, WriteState.WritingArray);
            if (_separatorRequired)
            {
                _sb.Append(',');
            }
        }


        public void WriteObjId(long objId)
        {
            AllowState(WriteState.WritingObject);
            _sb.Append("\"$id\":");
            _sb.Append(objId);
            _separatorRequired = true;
        }
        
        public void WriteObjRef(long objId)
        {
            AllowState(WriteState.WritingObject, WriteState.WritingArray);
            _sb.Append("\"$ref\":");
            _sb.Append(objId);
            _separatorRequired = true;
        }

        public void WriteNullValue()
        {
            AllowState(WriteState.WritingPropertyContent, WriteState.WritingArray);
            _sb.Append("null");
            _separatorRequired = true;
        }

        public void StartWriteArray()
        {    
            AllowState(WriteState.WritingPropertyContent, WriteState.WritingArray);
            SetState(WriteState.WritingArray);
            _level++;
            _levelTypes.Push(LevelType.Array);
            _separatorRequired = false;
            _sb.Append('[');
        }

        public void EndWriteArray()
        {
            AllowState(WriteState.WritingArray);
            _level--;
            _levelTypes.Pop();
            _separatorRequired = false;
            SetStateBackToPropertyContentOrArray();
            _sb.Append(']');
        }

        private void SetStateBackToObjectOrArray()
        {
            //we need to know whether what is the levelType now
            LevelType levelType = _levelTypes.Pop();
            if (levelType == LevelType.Object)
            {
                SetState(WriteState.WritingObject);
            }
            else
            {
                SetState(WriteState.WritingArray);
            }
            //push it back again
            _levelTypes.Push(levelType);
        }
        
        private void SetStateBackToPropertyContentOrArray()
        {
            //we need to know whether what is the levelType now
            LevelType levelType = _levelTypes.Pop();
            if (levelType == LevelType.Object)
            {
                SetState(WriteState.WritingPropertyContent);
            }
            else
            {
                SetState(WriteState.WritingArray);
            }
            //push it back again
            _levelTypes.Push(levelType);
        }
    }
}
