using System.Text.Json;

namespace ZoDream.Tests
{
    [TestClass]
    public sealed class JsonTest
    {
        [TestMethod]
        public void TestParse()
        {
            var text = "";
            using var doc = JsonDocument.Parse(text);


        }
    }
}
