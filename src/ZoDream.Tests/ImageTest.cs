using System.Text.Json;

namespace ZoDream.Tests
{
    [TestClass]
    public sealed class ImageTest
    {
        [TestMethod]
        public void TestRotate()
        {
            var w = 90;
            var h = 180;
            var angle = 270;
            var (a, b) = Shared.Drawing.SkiaExtension.ComputedRotate(w, h, angle);
            Assert.AreEqual(h, a);
            Assert.AreEqual(w, b);
        }
    }
}
