using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

namespace ContreJour.Browser
{
    internal static partial class HostEventInterop
    {
        public static Task ImportAsync()
        {
            return JSHost.ImportAsync("hostevents", "../host-events.js");
        }

        // Points the page's writer at the ring.
        [JSImport("attach", "hostevents")]
        public static partial void Attach(int address, int threadId);
    }
}
