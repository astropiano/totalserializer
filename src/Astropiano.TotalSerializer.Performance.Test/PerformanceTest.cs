using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Astropiano.TotalSerializer.Performance.Test
{
    [TestFixture]
    public class PerformanceTest
    {
        [Explicit("Work in progress")]
        [Test]
        public void TestMethod1()
        {
            int repetitions = 100;
            Stopwatch stopwatch = new Stopwatch();
            TotalSerializer serializer = new TotalSerializer();
            BigTestObject bigTestObject = BigTestObject.Create(1000);
            stopwatch.Restart();
            string[] jsons = new string[repetitions];
            for (int i = 0; i < repetitions; ++i)
            {
                jsons[i] = serializer.Serialize(bigTestObject);
            }
            stopwatch.Stop();
            Console.WriteLine("Serialization: " + stopwatch.Elapsed);
            stopwatch.Restart();
            BigTestObject[] deserializeds = new BigTestObject[repetitions];
            for (int i = 0; i < repetitions; ++i)
            {
                deserializeds[i] = serializer.Deserialize<BigTestObject>(jsons[i]);
            }
            stopwatch.Stop();
            Console.WriteLine("Deserialization: " + stopwatch.Elapsed);
        }
    }
}
