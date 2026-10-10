using System.Collections.Generic;
using System.Globalization;
using System.IO;

using Mokus2D;
using Mokus2D.Content;
using Mokus2D.Graphics;

namespace ContreJourDX.Gameplay
{
    public static class ClipFactory
    {
        public static readonly MokusContentManager content;

        private static readonly HashSet<string> missingScaledTextures = [];

        static ClipFactory()
        {
            content = Mokus2DGame.ContentManager;
        }

        public static ITexture GetTexture(string name)
        {
            string asset = $"Graphics/textures/{name}";
            float scaleFactor = Mokus2DGame.Config.GraphicsLoader.PrefferedScaleFactor;
            if (scaleFactor != 1f)
            {
                string scaled = $"{asset}.x{scaleFactor.ToString(CultureInfo.InvariantCulture)}.png";
                if (!missingScaledTextures.Contains(scaled))
                {
                    try
                    {
                        return content.Load(scaled);
                    }
                    catch (FileNotFoundException)
                    {
                        _ = missingScaledTextures.Add(scaled);
                    }
                }
            }
            return content.Load(asset);
        }
    }
}
