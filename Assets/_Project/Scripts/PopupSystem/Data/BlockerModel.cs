using System;
using R3;

namespace PopupShowcase.PopupSystem
{
    public interface IBlockerModel
    {
        ReadOnlyReactiveProperty<bool> IsVisible { get; }
    }

    public class BlockerModel : IBlockerModel, IDisposable
    {
        public ReadOnlyReactiveProperty<bool> IsVisible => _isVisible;

        private readonly ReactiveProperty<bool> _isVisible = new(false);

        public void SetVisible(bool isVisible)
        {
            _isVisible.Value = isVisible;
        }

        public void Dispose()
        {
            _isVisible.Dispose();
        }
    }
}
