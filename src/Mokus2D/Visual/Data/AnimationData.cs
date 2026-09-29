using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace Mokus2D.Visual.Data
{
    [Serializable]
    public class AnimationData : ConfigData
    {
        public Rectangle PrecalculatedBounds { get; set; }

        private Dictionary<string, Dictionary<string, string>> _instanceConfigs;

        private readonly List<List<AnimationFrameData>> _frames = [];

        public List<AnimationFrameData> this[int index] => _frames[index];

        public int Count => _frames.Count;

        public void AddInstanceConfig(string childName, Dictionary<string, string> config)
        {
            _instanceConfigs ??= [];
            _instanceConfigs[childName] = config;
        }

        public Dictionary<string, string> GetInstanceConfig(string childName)
        {
            return _instanceConfigs?.GetValueOrDefault(childName);
        }

        public AnimationFrameData GetChildFrameData(int frame, string childName)
        {
            List<AnimationFrameData> list = this[frame];
            foreach (AnimationFrameData item in list)
            {
                if (item.Id == childName)
                {
                    return item;
                }
            }
            return null;
        }

        public void Add(List<AnimationFrameData> item)
        {
            _frames.Add(item);
        }
    }
}
