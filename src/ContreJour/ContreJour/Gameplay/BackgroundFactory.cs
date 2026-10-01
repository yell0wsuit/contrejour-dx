using System.IO;

using Mokus2D.Visual;

namespace ContreJour.Gameplay
{
    public static class BackgroundFactory
    {
        public static BackgroundBase Create(string type, Node node, Hashtable config, ContreJourGame game)
        {
            return type switch
            {
                "BackgroundBase" => new BackgroundBase(node, config, game),
                "FadeBackground" => new FadeBackground(node, config, game),
                "LightPowerBackground" => new LightPowerBackground(node, config, game),
                "MoveBackground" => new MoveBackground(node, config, game),
                "RadiusRotatableBackground" => new RadiusRotatableBackground(node, config, game),
                "RotatableBackground" => new RotatableBackground(node, config, game),
                "SunBackground" => new SunBackground(node, config, game),
                _ => throw new InvalidDataException($"No background type '{type}'."),
            };
        }
    }
}
