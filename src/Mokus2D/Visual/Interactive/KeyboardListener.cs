using System;
using System.Collections.Generic;

using Mokus2D.Collections;
using Mokus2D.Input;
using Mokus2D.Util.Resources;

namespace Mokus2D.Visual.Interactive
{
    public class KeyboardListener : DisposableBase
    {
        private readonly FactoryDictionary<Key, List<Action<Key, bool>>> _keysList = new(k => []);

        public bool Enabled
        {
            get;
            set
            {
                if (!field)
                {
                    field = value;
                    if (field)
                    {
                        AddListeners();
                    }
                    else
                    {
                        RemoveListeners();
                    }
                }
            }
        } = true;

        private void AddListeners()
        {
            foreach (KeyValuePair<Key, List<Action<Key, bool>>> keys in _keysList)
            {
                foreach (Action<Key, bool> item in keys.Value)
                {
                    Mokus2DGame.Keyboard.AddListener(keys.Key, item);
                }
            }
        }

        private void RemoveListeners()
        {
            foreach (KeyValuePair<Key, List<Action<Key, bool>>> keys in _keysList)
            {
                foreach (Action<Key, bool> item in keys.Value)
                {
                    Mokus2DGame.Keyboard.RemoveListener(keys.Key, item);
                }
            }
        }

        public void AddListener(Key key, Action<Key, bool> listener)
        {
            _keysList.GetOrCreate(key).Add(listener);
            if (Enabled)
            {
                Mokus2DGame.Keyboard.AddListener(key, listener);
            }
        }

        public void RemoveListener(Key key, Action<Key, bool> listener)
        {
            _ = _keysList[key].Remove(listener);
            if (Enabled)
            {
                Mokus2DGame.Keyboard.RemoveListener(key, listener);
            }
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            Enabled = false;
        }
    }
}
