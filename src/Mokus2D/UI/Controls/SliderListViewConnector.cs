using System;

using Mokus2D.UI.Grids;
using Mokus2D.Util.MathUtils;
using Mokus2D.Visual;

namespace Mokus2D.UI.Controls
{
    public class SliderListViewConnector
    {
        private readonly Slider _slider;

        private readonly IListView _listView;

        public Sprite MouseWheelArea
        {
            set => _slider.MouseWheelArea = value;
        }

        public SliderListViewConnector(Slider slider, IListView listView)
        {
            _slider = slider;
            _listView = listView;
            _slider.ChangeEvent += OnSliderChange;
            _listView.DataChangedEvent += OnDataChanged;
        }

        private void OnDataChanged()
        {
            _slider.Max = Math.Max(_listView.DataCount - _listView.ItemsCount, 0);
            _listView.ItemsPosition = _listView.ItemsPosition.Clamp(_slider.Min, _slider.Max);
        }

        private void OnSliderChange(Slider obj)
        {
            _listView.ItemsPosition = _slider.Value;
        }

        public void RefreshListPosition()
        {
            _listView.ItemsPosition = _slider.Value;
        }
    }
}
