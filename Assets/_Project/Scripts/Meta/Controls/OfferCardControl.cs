using PopupShowcase.Core;
using PopupShowcase.Meta.Models;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace PopupShowcase.Meta.Controls
{
    public class OfferCardControl : BaseControl<IOfferCardModel>
    {
        [SerializeField] private ExtendedImage _offerImage;
        [SerializeField] private ExtendedText _offerText;
        [SerializeField] private Button _cardButton;

        protected override void OnModelUpdate(CompositeDisposable modelBindings)
        {
            _offerText.text = Model.Text;
            _offerImage.LoadSpriteAsync(Model.IconPath);

            _cardButton.onClick.AsObservable()
                .Subscribe(_ => Model.Select())
                .AddTo(modelBindings);
        }

        protected override void OnUnbind()
        {
            _offerImage.Clear();
            base.OnUnbind();
        }

        protected override void OnDestroy()
        {
            _offerImage.Clear();
            base.OnDestroy();
        }
    }
}
