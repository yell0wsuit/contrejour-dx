using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Microsoft.Extensions.Logging;

namespace Mokus2D.Diagnostics
{
    // The one shape a written log entry takes, in the file and on the console, with the account
    // name taken out: save paths and every stack frame carry the home folder, which names the
    // person who sends the log.
    public static class LogEntryFormat
    {
        public const string RedactedUserName = "[redactedUsername]";

        // A one or two letter name occurs inside ordinary words.
        private const int ShortestRedactableName = 3;

        // Every spelling of the account name, longest first so a shorter prefix cannot eat a match:
        // the profile folder can differ from the account after a rename, and Windows hands back 8.3
        // aliases (YELL0W~1) for long names.
        private static readonly string[] UserNames = ResolveUserNames();

        // The exception, when there is one, goes on lines of its own.
        public static string Compose(LogLevel level, string category, string message, Exception exception)
        {
            StringBuilder line = new();
            _ = line.Append(DateTime.Now.ToString("O", CultureInfo.InvariantCulture))
                .Append(" [")
                .Append(level)
                .Append("] ")
                .Append(category)
                .Append(' ')
                .Append(message);
            if (exception != null)
            {
                _ = line.AppendLine().Append(exception);
            }
            return Redact(line.ToString());
        }

        public static string Redact(string text)
        {
            if (text == null)
            {
                return text;
            }
            string redacted = text;
            foreach (string name in UserNames)
            {
                // Case-insensitive: Windows paths reach the log in whatever case their source used.
                redacted = redacted.Replace(name, RedactedUserName, StringComparison.OrdinalIgnoreCase);
            }
            return redacted;
        }

        private static string[] ResolveUserNames()
        {
            List<string> names = [];
            try
            {
                Add(Environment.UserName);
                Add(LeafOf(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile)));
            }
            catch (PlatformNotSupportedException)
            {
                return [];
            }
            foreach (string name in names.ToArray())
            {
                if (name.Length > 8)
                {
                    Add(name[..6] + "~");
                }
            }
            names.Sort(static (left, right) => right.Length.CompareTo(left.Length));
            return [.. names];

            void Add(string name)
            {
                if (!string.IsNullOrWhiteSpace(name)
                    && name.Length >= ShortestRedactableName
                    && !names.Contains(name, StringComparer.OrdinalIgnoreCase))
                {
                    names.Add(name);
                }
            }
        }

        private static string LeafOf(string path)
        {
            return string.IsNullOrEmpty(path)
                ? null
                : new DirectoryInfo(path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)).Name;
        }
    }
}
