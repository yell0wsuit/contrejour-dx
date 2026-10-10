using System;

namespace ContreJourDX.Browser.Platform
{
    // The game's two-letter language: ?lang=xx first (for trying a locale), then the browser's language, then
    // English. The runtime has no culture data (invariant globalization), so the host has to say.
    public static class BrowserLanguage
    {
        public const string Fallback = "en";

        public static string Parse(string query, string navigatorLanguage)
        {
            return TwoLetters(QueryValue(query, "lang")) ?? TwoLetters(navigatorLanguage) ?? Fallback;
        }

        private static string QueryValue(string query, string name)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }
            foreach (string pair in query.TrimStart('?').Split('&'))
            {
                int equals = pair.IndexOf('=', StringComparison.Ordinal);
                if (equals > 0 && pair[..equals] == name)
                {
                    return Uri.UnescapeDataString(pair[(equals + 1)..]);
                }
            }
            return null;
        }

        // "zh-Hant-TW" -> "zh", "pt_BR" -> "pt"; anything whose first subtag is not exactly two ASCII letters is
        // rejected.
        private static string TwoLetters(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return null;
            }
            string first = tag.Split('-', '_')[0];
            return first.Length == 2 && char.IsAsciiLetter(first[0]) && char.IsAsciiLetter(first[1])
                ? first.ToLowerInvariant()
                : null;
        }
    }
}
