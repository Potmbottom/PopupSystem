using R3;

namespace PopupShowcase.MVVM.Popups.Models
{
    public class OfferPopupModel : BasePopupModel
    {
        public override PopupType Type => PopupType.Offer;
        public string ItemName { get; }
        public string Description { get; }
        public string ItemIconPath { get; }
        public Observable<Unit> BuyRequested => _buyRequested;

        private readonly Subject<Unit> _buyRequested = new();

        public OfferPopupModel(string itemName, string description, string itemIconPath)
            : base(PopupPriority.Offer)
        {
            ItemName = itemName;
            Description = description;
            ItemIconPath = itemIconPath;
        }

        public void RequestBuy()
        {
            _buyRequested.OnNext(Unit.Default);
        }

        public override void Dispose()
        {
            _buyRequested.Dispose();
            base.Dispose();
        }
    }
}
