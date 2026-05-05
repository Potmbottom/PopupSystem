using PopupShowcase.MVVM.ViewModels;

namespace PopupShowcase.MVVM.Models
{
    public interface IOfferCardModel
    {
        string Text { get; }
        string IconPath { get; }
        void Select();
    }

    public class OfferCardModel : IOfferCardModel
    {
        public string Text { get; }
        public string IconPath { get; }

        private readonly OffersModel _offersModel;
        private readonly string _offerId;

        public OfferCardModel(OffersModel offersModel, OfferModel offer)
        {
            _offersModel = offersModel;
            _offerId = offer.OfferId;
            Text = offer.CardText;
            IconPath = offer.IconPath;
        }

        public void Select()
        {
            _offersModel.SelectOffer(_offerId);
        }
    }
}
