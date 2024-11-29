using SkiaSharp;
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
            var m = SKMatrix.CreateRotationDegrees(angle);
            var p = m.MapPoint(w, h);
            Assert.AreEqual(h, a);
            Assert.AreEqual(w, b);
            Assert.AreEqual(h, p.X);
            Assert.AreEqual(w, p.Y);
        }
    }
}
