using PopupShowcase.PopupSystem;

namespace PopupShowcase.Meta.Popups
{
    public class SystemInterruptPopupData : BasePopupData
    {
        public override PopupType Type => PopupType.SystemInterrupt;
        public string Message { get; }

        public SystemInterruptPopupData(string message)
            : base(PopupPriority.SystemInterrupt)
        {
            Message = message;
        }
    }
}
