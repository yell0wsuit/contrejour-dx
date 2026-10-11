using System.IO;

namespace ContreJourDX.Saving
{
    /// <summary>Preference storage backed by JSON files in the save directory.</summary>
    /// <param name="directory">Absolute path to the save directory.</param>
    internal sealed class FilePreferenceStore(string directory) : IPreferenceStore
    {
        /// <inheritdoc />
        public string Read(string name)
        {
            string path = Path.Combine(directory, name);
            return File.Exists(path) ? File.ReadAllText(path) : null;
        }

        /// <inheritdoc />
        public void Write(string name, string contents)
        {
            _ = Directory.CreateDirectory(directory);
            File.WriteAllText(Path.Combine(directory, name), contents);
        }
    }
}
