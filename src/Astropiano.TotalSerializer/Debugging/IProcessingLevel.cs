using System;

namespace Astropiano.TotalSerializer.Debug
{
    public interface IProcessingLevel
    {
        string GetDebugInfo();

        Type TryGetObjectType();
    }
}