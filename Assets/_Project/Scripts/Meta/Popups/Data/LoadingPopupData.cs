using PopupShowcase.PopupSystem;
using R3;

namespace PopupShowcase.Meta.Popups
{
    public class LoadingPopupData : BasePopupData
    {
        public override PopupType Type => PopupType.Loading;
        public ReactiveProperty<float> Progress { get; } = new(0f);

        public LoadingPopupData() : base(PopupPriority.SystemInterrupt)
        {
        }

        public override void Dispose()
        {
            Progress.Dispose();
            base.Dispose();
        }
    }
}
