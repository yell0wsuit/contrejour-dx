using System.IO;

namespace Mokus2D.Util
{
    public static class PathUtil
    {
        public static string Combine(params string[] parts)
        {
            string text = parts[0];
            for (int i = 1; i < parts.Length; i++)
            {
                text = Path.Combine(
                [
                    text,
                    parts[i]
                ]);
            }
            return text;
        }
    }
}
