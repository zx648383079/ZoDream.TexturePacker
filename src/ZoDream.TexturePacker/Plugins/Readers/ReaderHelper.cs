namespace ZoDream.TexturePacker.Plugins.Readers
{
    public static class ReaderHelper
    {
        public static string MatchWithRange(string text, string begin, string end)
        {
            var i = text.IndexOf(begin);
            if (i < 0)
            {
                return string.Empty;
            }
            i += begin.Length;
            var j = text.IndexOf(end, i);
            if (j < 0)
            {
                return text[i..].Trim();
            }
            return text[i..j].Trim();
        }

        public static int TryParseInt(string value)
        {
            value = value.Trim();
            var i = value.IndexOf('.');
            if (i >= 0)
            {
                value = value[..i];
            }
            if (int.TryParse(value, out i))
            {
                return i;
            }
            return 0;
        }
    }
}
