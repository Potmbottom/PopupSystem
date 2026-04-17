using PopupShowcase.PopupSystem;

namespace PopupShowcase.Meta.Popups
{
    public class DailyRewardPopupData : BasePopupData
    {
        public override PopupType Type => PopupType.DailyReward;
        public string Description { get; }

        public DailyRewardPopupData(string description)
            : base(PopupPriority.Standard)
        {
            Description = description;
        }
    }
}
