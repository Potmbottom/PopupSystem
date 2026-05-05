
namespace PopupShowcase.MVVM.Popups.Models
{
    public class TutorialCompletePopupModel : BasePopupModel
    {
        public override PopupType Type => PopupType.TutorialComplete;
        public string Title { get; }
        public string Description { get; }

        public TutorialCompletePopupModel(string title, string description)
            : base(PopupPriority.Standard)
        {
            Title = title;
            Description = description;
        }
    }
}
