using FluentAssertions;
using Xunit;
using Astropiano.TotalSerializer.CharArray;

namespace Astropiano.TotalSerializer.Test.CharArray
{
    public class JsonScopeArrayAnalyzerTest
    {
        [Fact]
        public void ShouldFindTheEndOfAnObjectScopeGivenTheIndexOfTheScopeStart()
        {
            string json = "{\"MyInt\":123,\"MyObject\":{\"MyDouble\":3.14159}}";
            //number:      0 123456 789
            //                        10  13     20 22  25   30    35       44

            char[] charArray = json.ToCharArray();
            
            JsonScopeAnalyzer jsonScopeAnalyzer = new JsonScopeAnalyzer(charArray);

            jsonScopeAnalyzer.FindScopeEnd(0).Should().Be(44);
            jsonScopeAnalyzer.FindScopeEnd(24).Should().Be(43);
        }
        
        [Fact]
        public void ShouldFindTheEndOfAnArrayScopeGivenTheIndexOfTheScopeStart()
        {
            string json = "{\"MyInt\":123,\"MyArray\":[1,2,3,{\"item\":\"cup\"}]}";
            //number:      0 123456 789
            //                        10  13     20 22 25   30    35     40    45

            char[] charArray = json.ToCharArray();
            
            JsonScopeAnalyzer jsonScopeAnalyzer = new JsonScopeAnalyzer(charArray);

            jsonScopeAnalyzer.FindScopeEnd(0).Should().Be(45);
            jsonScopeAnalyzer.FindScopeEnd(23).Should().Be(44);
            jsonScopeAnalyzer.FindScopeEnd(30).Should().Be(43);
        }
        
        [Fact]
        public void ShouldFindTheEndOfAScopeGivenAnyIndexInAScope()
        {
            string json = "{\"MyInt\":123,\"MyArray\":[1,2,3,{\"item\":\"cup\"}]}";
            //number:      0 123456 789
            //                        10  13     20 22 25   30    35     40    45

            char[] charArray = json.ToCharArray();
            
            JsonScopeAnalyzer jsonScopeAnalyzer = new JsonScopeAnalyzer(charArray);

            jsonScopeAnalyzer.FindScopeEnd(2).Should().Be(45);
            jsonScopeAnalyzer.FindScopeEnd(15).Should().Be(45);
            jsonScopeAnalyzer.FindScopeEnd(26).Should().Be(44);
            jsonScopeAnalyzer.FindScopeEnd(33).Should().Be(43);
        }

        [Fact]
        public void IgnoreStringsInJson()
        {
            string json = "{\"MyInt\":123,\"MyArray\":[1,2,3,{\"item\":\"[]}\"}]}";
            //number:      0 123456 7 10  13     20 22 25   30    35     40    45

            char[] charArray = json.ToCharArray();
            
            JsonScopeAnalyzer jsonScopeAnalyzer = new JsonScopeAnalyzer(charArray);

            jsonScopeAnalyzer.FindScopeEnd(2).Should().Be(45);
            jsonScopeAnalyzer.FindScopeEnd(15).Should().Be(45);
            jsonScopeAnalyzer.FindScopeEnd(26).Should().Be(44);
            jsonScopeAnalyzer.FindScopeEnd(33).Should().Be(43);
        }
        
        [Fact]
        public void CanGetSubScope()
        {
            string json = "{\"MyObj\":{\"text\":\"Hello World!\"}}";
            //number:      0 123456 7  10    15    20   25    30
            
            string subJson = "{\"text\":\"Hello World!\"}";
            //number:         0 12345 67 8 11  15   20 22

            char[] charArray = json.ToCharArray();
            
            JsonScopeAnalyzer jsonScopeAnalyzer = new JsonScopeAnalyzer(charArray);

            JsonScopeAnalyzer subScopeAnalyzer = jsonScopeAnalyzer.GetSubScope(9, 31);
            
            subScopeAnalyzer.FindScopeEnd(0).Should().Be(22);
        }
        
        
    }
}