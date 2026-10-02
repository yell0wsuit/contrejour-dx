using System;

using Mokus2D.Visual;

namespace ContreJour.Content
{
    public sealed class ClipEntry(string id, ClipKind kind, Func<Node> composite = null)
    {
        public string Id { get; } = id;

        public ClipKind Kind { get; } = kind;

        public Node Create()
        {
            return Kind switch
            {
                ClipKind.Sprite => new Sprite(Id),
                ClipKind.MovieClip => new MovieClip(Id),
                ClipKind.Composite => composite(),
                _ => throw new InvalidOperationException($"Unknown clip kind {Kind}."),
            };
        }
    }
}
