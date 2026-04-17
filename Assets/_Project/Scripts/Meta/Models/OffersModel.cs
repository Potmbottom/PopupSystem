using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace PopupShowcase.Meta.Models
{
    public interface IOffersModel
    {
        ReadOnlyReactiveProperty<IReadOnlyList<IOfferCardModel>> VisibleOffers { get; }
        Observable<string> OfferSelected { get; }
        bool TryGetOffer(string offerId, out OfferStateModel offer);
    }

    public class OffersModel : IOffersModel, IDisposable
    {
        public ReadOnlyReactiveProperty<IReadOnlyList<IOfferCardModel>> VisibleOffers => _visibleOffers;
        public Observable<string> OfferSelected => _offerSelected;

        private readonly OfferCatalogConfig _offerCatalogConfig;
        private readonly PlayerStateModel _playerState;
        private readonly ReactiveProperty<IReadOnlyList<IOfferCardModel>> _visibleOffers = new(Array.Empty<IOfferCardModel>());
        private readonly Subject<string> _offerSelected = new();
        private readonly Dictionary<string, OfferStateModel> _offersById = new();
        private readonly CompositeDisposable _disposables = new();

        public OffersModel(OfferCatalogConfig offerCatalogConfig, PlayerStateModel playerState)
        {
            _offerCatalogConfig = offerCatalogConfig;
            _playerState = playerState;

            _playerState.OffersChanged
                .Subscribe(_ => Refresh())
                .AddTo(_disposables);

            Refresh();
        }

        public bool TryGetOffer(string offerId, out OfferStateModel offer)
        {
            return _offersById.TryGetValue(offerId, out offer);
        }

        public void SelectOffer(string offerId)
        {
            if (string.IsNullOrWhiteSpace(offerId))
                return;

            if (_offersById.ContainsKey(offerId))
                _offerSelected.OnNext(offerId);
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _visibleOffers.Dispose();
            _offerSelected.Dispose();
        }

        private void Refresh()
        {
            var visibleOffers = new List<IOfferCardModel>();
            _offersById.Clear();

            foreach (var offer in _offerCatalogConfig.Offers)
            {
                if (offer == null || string.IsNullOrWhiteSpace(offer.OfferId))
                    continue;

                if (!_playerState.IsOfferActive(offer.OfferId))
                    continue;

                if (offer.RequiresTutorialComplete && !_playerState.TutorialCompleted.CurrentValue)
                    continue;

                if (_playerState.IsOfferPurchased(offer.OfferId))
                    continue;

                var offerModel = new OfferStateModel(
                    offer.OfferId,
                    offer.CardText,
                    offer.PopupTitle,
                    offer.PopupDescription,
                    offer.IconPath);

                _offersById[offerModel.OfferId] = offerModel;
                visibleOffers.Add(new OfferCardModel(this, offerModel));
            }

            _visibleOffers.Value = visibleOffers;
        }
    }
}
