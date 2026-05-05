using System.Collections.Generic;
using PopupShowcase.MVVM.Common;
using PopupShowcase.MVVM.Models;
using PopupShowcase.MVVM.ViewModels;
using R3;
using UnityEngine;

namespace PopupShowcase.MVVM.Views
{
    public class OffersShowView : BaseView<IOffersModel>
    {
        [SerializeField] private OfferCardView _offerCard1;
        [SerializeField] private OfferCardView _offerCard2;

        protected override void OnModelUpdate(CompositeDisposable modelBindings)
        {
            Model.VisibleOffers
                .Subscribe(ApplyOffers)
                .AddTo(modelBindings);
        }

        protected override void OnUnbind()
        {
            UnbindCard(_offerCard1);
            UnbindCard(_offerCard2);
            base.OnUnbind();
        }

        private void ApplyOffers(IReadOnlyList<IOfferCardModel> offers)
        {
            ApplyOffer(_offerCard1, offers, 0);
            ApplyOffer(_offerCard2, offers, 1);
        }

        private void ApplyOffer(OfferCardView card, IReadOnlyList<IOfferCardModel> offers, int index)
        {
            if (card == null)
                return;

            if (offers == null || index >= offers.Count)
            {
                card.UnbindAndDisableGameObject();
                return;
            }

            card.BindAndSetActive(offers[index]);
        }

        private static void UnbindCard(OfferCardView card)
        {
            if (card != null)
                card.UnbindAndDisableGameObject();
        }
    }
}
