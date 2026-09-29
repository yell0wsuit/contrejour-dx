using System;

using Mokus2D.Util;
using Mokus2D.Visual;

namespace Mokus2D.UI.Controls
{
    public abstract class UIComponent : Node
    {
        private readonly Flag _propertiesDirty = new();

        public event Action PropertiesUpdatedEvent;

        protected void SetPropertiesDirty()
        {
            _propertiesDirty.SetOn();
        }

        public override void Update(float time)
        {
            base.Update(time);
            if (_propertiesDirty.Use())
            {
                UpdateProperties();
                PropertiesUpdatedEvent.Dispatch();
            }
        }

        protected virtual void UpdateProperties()
        {
        }
    }
}
