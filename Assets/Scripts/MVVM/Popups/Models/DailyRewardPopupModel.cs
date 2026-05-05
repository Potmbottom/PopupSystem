
namespace PopupShowcase.MVVM.Popups.Models
{
    public class DailyRewardPopupModel : BasePopupModel
    {
        public override PopupType Type => PopupType.DailyReward;
        public string Description { get; }

        public DailyRewardPopupModel(string description)
            : base(PopupPriority.Standard)
        {
            Description = description;
        }
    }
}
