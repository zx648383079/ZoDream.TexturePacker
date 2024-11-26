using System.Text;
using System.Text.Json;

namespace ZoDream.Tests
{
    [TestClass]
    public sealed class BinaryTest
    {
        [TestMethod]
        public void TestRead()
        {
            var buffer = new byte[] { 100, 0, };
            var text = Encoding.UTF8.GetString(buffer);
            Assert.IsTrue(text.Length == 2);
        }
    }
}
