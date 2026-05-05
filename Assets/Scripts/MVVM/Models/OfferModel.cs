namespace PopupShowcase.MVVM.Models
{
    public class OfferModel
    {
        public string OfferId { get; }
        public string CardText { get; }
        public string PopupTitle { get; }
        public string PopupDescription { get; }
        public string IconPath { get; }

        public OfferModel(
            string offerId,
            string cardText,
            string popupTitle,
            string popupDescription,
            string iconPath)
        {
            OfferId = offerId;
            CardText = cardText;
            PopupTitle = popupTitle;
            PopupDescription = popupDescription;
            IconPath = iconPath;
        }
    }
}
