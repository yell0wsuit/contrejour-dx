namespace ContreJour.Saving
{
    /// <summary>
    /// Named-blob persistence behind <see cref="Preferences"/>.
    /// </summary>
    /// <remarks>
    /// Deliberately synchronous: preferences are read and written from ordinary game code with no
    /// await points available.
    /// </remarks>
    internal interface IPreferenceStore
    {
        /// <summary>Reads a stored blob, or <see langword="null"/> when it is absent.</summary>
        /// <param name="name">Blob name, e.g. <c>contrejour_preferences.json</c>.</param>
        string Read(string name);

        /// <summary>Writes a blob, replacing any existing value.</summary>
        /// <param name="name">Blob name.</param>
        /// <param name="contents">Serialized contents.</param>
        void Write(string name, string contents);
    }
}
