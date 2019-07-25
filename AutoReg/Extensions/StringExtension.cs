using Microsoft.Xna.Framework;

namespace PiQiu.AutoReg.Extensions
{
    internal static class StringExtension
    {
        private const string Tag = "[c/{1}{2}{3}:{0}]";

        public static string ColorTag(this string msg, Color color)
        {
            var r = color.R.ToString("x2");
            var g = color.G.ToString("x2");
            var b = color.B.ToString("x2");
            var tag = string.Format(Tag, msg, r, g, b);
            return tag;
        }
    }
}
