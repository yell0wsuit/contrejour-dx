using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace ContreJour.Browser
{
    internal static partial class LogInterop
    {
        public static Task ImportAsync()
        {
            return JSHost.ImportAsync("log", "../log.js");
        }

        [JSImport("begin", "log")]
        public static partial string Begin(string header);

        [JSImport("append", "log")]
        public static partial void Append(string line, bool urgent);
    }
}
