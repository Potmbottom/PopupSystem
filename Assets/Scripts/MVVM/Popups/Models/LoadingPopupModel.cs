
namespace PopupShowcase.MVVM.Popups.Models
{
    public class LoadingPopupModel : BasePopupModel
    {
        public override PopupType Type => PopupType.Loading;

        public LoadingPopupModel() : base(PopupPriority.SystemInterrupt)
        {
        }
    }
}
