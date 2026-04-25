using PopupShowcase.PopupSystem;

namespace PopupShowcase.Meta.Popups
{
    public class LoadingPopupData : BasePopupData
    {
        public override PopupType Type => PopupType.Loading;

        public LoadingPopupData() : base(PopupPriority.SystemInterrupt)
        {
        }
    }
}
