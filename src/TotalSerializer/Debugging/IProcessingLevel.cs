using System;

namespace TotalSerializer.Debug
{
    public interface IProcessingLevel
    {
        string GetDebugInfo();

        Type TryGetObjectType();
    }
}