
namespace PopupShowcase.MVVM.Popups.Models
{
    public class SystemInterruptPopupModel : BasePopupModel
    {
        public override PopupType Type => PopupType.SystemInterrupt;
        public string Message { get; }

        public SystemInterruptPopupModel(string message)
            : base(PopupPriority.SystemInterrupt)
        {
            Message = message;
        }
    }
}
