using System;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using TotalSerializer.Performance.Test;

namespace TotalSerializer.Comparison
{
    [TestFixture]
    public class SerializerBenchmark
    {
        private const int Warmup = 5;
        private const int Repetitions = 50;

        [Test]
        public void CompareSerializers()
        {
            BigTestObject testObject = BigTestObject.Create(1000);
            StringBuilder report = new StringBuilder();
            report.AppendLine();
            report.AppendLine($"{"Serializer",-25} {"Serialize",-12} {"Deserialize",-12} {"Payload",-10}");
            report.AppendLine(new string('-', 62));

            Report(report, "TotalSerializer",
                () => new TotalSerializer().Serialize(testObject),
                json => new TotalSerializer().Deserialize<BigTestObject>(json));

            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            };
            Report(report, "Newtonsoft.Json (TNH.All)",
                () => JsonConvert.SerializeObject(testObject, settings),
                json => JsonConvert.DeserializeObject<BigTestObject>(json, settings));

            Report(report, "System.Text.Json",
                () => System.Text.Json.JsonSerializer.Serialize(testObject),
                json => System.Text.Json.JsonSerializer.Deserialize<BigTestObject>(json));

            Assert.Pass(report.ToString());
        }

        private static void Report(StringBuilder report, string name, Func<string> serialize, Func<string, object> deserialize)
        {
            for (int i = 0; i < Warmup; ++i)
            {
                deserialize(serialize());
            }

            string[] payloads = new string[Repetitions];
            Stopwatch stopwatch = Stopwatch.StartNew();
            for (int i = 0; i < Repetitions; ++i)
            {
                payloads[i] = serialize();
            }
            stopwatch.Stop();
            TimeSpan serialization = stopwatch.Elapsed;

            stopwatch.Restart();
            foreach (string payload in payloads)
            {
                deserialize(payload);
            }
            stopwatch.Stop();
            TimeSpan deserialization = stopwatch.Elapsed;

            report.AppendLine($"{name,-25} {serialization.TotalMilliseconds / Repetitions,10:F2}ms {deserialization.TotalMilliseconds / Repetitions,10:F2}ms {payloads[0].Length,8}B");
        }
    }
}
