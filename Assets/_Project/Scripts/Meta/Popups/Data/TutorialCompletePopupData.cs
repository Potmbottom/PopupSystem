using PopupShowcase.PopupSystem;

namespace PopupShowcase.Meta.Popups
{
    public class TutorialCompletePopupData : BasePopupData
    {
        public override PopupType Type => PopupType.TutorialComplete;
        public string Title { get; }
        public string Description { get; }

        public TutorialCompletePopupData(string title, string description)
            : base(PopupPriority.Standard)
        {
            Title = title;
            Description = description;
        }
    }
}
