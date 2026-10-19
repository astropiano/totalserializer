using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Astropiano.TotalSerializer.Logger;

namespace Astropiano.TotalSerializer.Debug
{
    public static class ProcessingStackDebugReporter
    {
        private static HashSet<Type> _reportedTypes = new HashSet<Type>();
        
        private static ITotalSerializerLogger _logger = LoggerFactory.Create(typeof(ProcessingStackDebugReporter));
        
        /// <summary>
        /// Walks down the stack and reports the tree up to the first public type with a public constructor.
        /// This should help developers to track down types that need a custom serialization.
        /// </summary>
        public static void ReportUnsupportedTypeByStack(Stack<IProcessingLevel> processingStack)
        {
            Stack<IProcessingLevel> intermediateStack = new Stack<IProcessingLevel>();
                   
            StringBuilder debugStringBuilder= new StringBuilder();

            int consistencyCheckCount = processingStack.Count;

            IProcessingLevel processingLevel;

            bool alreadyReported = false;
            
            while (processingStack.Count > 0)
            {
                processingLevel = processingStack.Pop();

                if (processingStack.Count > 1)
                {
                    debugStringBuilder.Append(" ← ");    
                }
                
                debugStringBuilder.Append(processingLevel.GetDebugInfo());

                intermediateStack.Push(processingLevel);

                Type relevantType = processingLevel.TryGetObjectType();
                if (relevantType != null)
                {
                    if (_reportedTypes.Contains(relevantType))
                    {
                        alreadyReported = true;
                        break;   
                    }
                    if (relevantType.IsPublic)
                    {
                        var constructors = relevantType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                        if (constructors.Length > 0)
                        {
                            _reportedTypes.Add(relevantType);
                            break;
                        }
                    }
                }
            }

            if (!alreadyReported)
            {
                _logger.Warn("Problematic type tree: " + debugStringBuilder);
            }

            // push back to real stack
            while (intermediateStack.Count > 0)
            {
                processingLevel = intermediateStack.Pop();
                processingStack.Push(processingLevel);
            }

            if (processingStack.Count != consistencyCheckCount)
            {
                throw new Exception("Consistency check failed. This indicates a bug.");
            }
        }
    }
}