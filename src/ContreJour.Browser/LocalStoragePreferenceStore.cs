using System.IO;
using System.Runtime.InteropServices.JavaScript;

using ContreJour.Saving;

namespace ContreJour.Browser
{
    // The save files in localStorage, one key each under "contrejour/". A refused read or write (storage
    // disabled, quota) surfaces as IOException: Preferences then starts the file empty, or retries the save
    // with its backoff and finally drops it.
    internal sealed class LocalStoragePreferenceStore : IPreferenceStore
    {
        private const string Prefix = "contrejour/";

        public string Read(string name)
        {
            try
            {
                return StorageInterop.Read(Prefix + name);
            }
            catch (JSException failure)
            {
                throw new IOException($"Could not read {name} from localStorage: {failure.Message}", failure);
            }
        }

        public void Write(string name, string contents)
        {
            try
            {
                StorageInterop.Write(Prefix + name, contents);
            }
            catch (JSException failure)
            {
                throw new IOException($"Could not write {name} to localStorage: {failure.Message}", failure);
            }
        }
    }
}
