using System;
using System.Collections.Generic;
using Astropiano.TotalSerializer.Exceptions;

namespace Astropiano.TotalSerializer.CharArray
{
    
    public ref struct JsonScopeAnalyzer
    {
        private char[] _json;
        private Span<int> _scopeEnds;
        private bool _isInitialized;
        private int _offset;

        public JsonScopeAnalyzer(char[] json) : this()
        {
            _json = json;
        }
        
        private JsonScopeAnalyzer(JsonScopeAnalyzer parent, int start, int end) : this()
        {
            parent.EnsureIsInitialized();
            _scopeEnds = parent._scopeEnds.Slice(start, end - start);
            _isInitialized = true;
            _offset = start;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>index at the start or inside the scope of which we are searching for the end</returns>
        public double FindScopeEnd(int index)
        {
            EnsureIsInitialized();
            return _scopeEnds[index] - _offset;
        }

        private enum ScopeType
        {
            Object,
            Array
        }

        private void EnsureIsInitialized()
        {
            if (_isInitialized)
            {
                return;
            }

            BuildData();
            _isInitialized = true;
        }

        public int[] GetScopeEnds()
        {
            return _scopeEnds.ToArray(); //create a copy so nobody can manipulate
        }

        private void BuildData()
        {
            _scopeEnds = new int[_json.Length];

            bool inDoubleQuoteStringScope = false;


            Stack<int> scopeStarts = new Stack<int>();
            Stack<ScopeType> scopeTypes = new Stack<ScopeType>();

            int scopeStart;
            
            for (int i = 0; i < _json.Length; i++)
            {
                _scopeEnds[i] = 0; //initialize to 0
                char character = _json[i];
                if (character == '"')
                {
                    //switch string scope
                    inDoubleQuoteStringScope = !inDoubleQuoteStringScope;
                }

                if (!inDoubleQuoteStringScope)
                {
                    if (character == '{')
                    {
                        scopeTypes.Push(ScopeType.Object);
                        scopeStarts.Push(i);
                    }
                    else if (character == '}')
                    {
                        ScopeType scopeType = scopeTypes.Pop();
                        if (scopeType != ScopeType.Object)
                        {
                            throw new JsonFormatException($"Expected }} at index {i}");
                        }

                        scopeStart = scopeStarts.Pop();
                        _scopeEnds[scopeStart] = i; // mark start
                        _scopeEnds[i] = i; // mark end
                    }
                    else if (character == '[')
                    {
                        scopeTypes.Push(ScopeType.Array);
                        scopeStarts.Push(i);
                    }
                    else if (character == ']')
                    {
                        ScopeType scopeType = scopeTypes.Pop();
                        if (scopeType != ScopeType.Array)
                        {
                            throw new JsonFormatException($"Expected ] at index {i}");
                        }

                        scopeStart = scopeStarts.Pop();
                        _scopeEnds[scopeStart] = i; // mark start
                        _scopeEnds[i] = i; // mark end
                    }    
                }
            }

            int currentScopeEnd = 0;
            Stack<int> scopeEnds = new Stack<int>();
            for (int i = 0; i < _scopeEnds.Length; i++)
            {
                if (_scopeEnds[i] != 0)
                {
                    if (currentScopeEnd == 0)
                    {
                        //reached first scope start
                        scopeEnds.Push(currentScopeEnd);
                        currentScopeEnd = _scopeEnds[i];
                    }else if (currentScopeEnd == _scopeEnds[i])
                    {
                        //reached the end of the scope
                        currentScopeEnd = scopeEnds.Pop(); //go back to previous
                    }
                    else if (currentScopeEnd != _scopeEnds[i])
                    {
                        //reached a new scope
                        scopeEnds.Push(currentScopeEnd);
                        currentScopeEnd = _scopeEnds[i];
                        
                    }
                }
                else
                {
                    _scopeEnds[i] = currentScopeEnd;
                }
            }

        }

        public JsonScopeAnalyzer GetSubScope(int start, int end)
        {
            return new JsonScopeAnalyzer(this, start, end);
        }
    }
}