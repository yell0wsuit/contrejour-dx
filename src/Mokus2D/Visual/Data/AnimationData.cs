using System;
using System.Collections.Generic;

using Mokus2D.Util.Data;

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

        public AnimationData Clone()
        {
            AnimationData clone = new()
            {
                PrecalculatedBounds = PrecalculatedBounds,
                Config = Config is null ? null : new Dictionary<string, string>(Config),
            };
            if (_instanceConfigs != null)
            {
                clone._instanceConfigs = [];
                foreach (KeyValuePair<string, Dictionary<string, string>> pair in _instanceConfigs)
                {
                    clone._instanceConfigs[pair.Key] = pair.Value is null ? null : new Dictionary<string, string>(pair.Value);
                }
            }
            foreach (List<AnimationFrameData> frame in _frames)
            {
                clone._frames.Add(frame?.ConvertAll(static data => data?.Clone()));
            }
            return clone;
        }
    }
}
