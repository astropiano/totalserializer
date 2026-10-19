using Astropiano.TotalSerializer.Interfaces;

namespace Astropiano.TotalSerializer.StringReader
{
    public class DirectStringReader : IStringReader
    {
        
        
        public DirectStringReader(string json)
        {
            
        }
        
        public char this[int index] => throw new System.NotImplementedException();
    }
}