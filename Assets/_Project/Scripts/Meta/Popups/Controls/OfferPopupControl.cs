using PopupShowcase.Core;
using PopupShowcase.PopupSystem;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace PopupShowcase.Meta.Popups
{
    public class OfferPopupControl : BasePopupControl<OfferPopupData>
    {
        [SerializeField] private ExtendedText _itemNameText;
        [SerializeField] private ExtendedText _descriptionText;
        [SerializeField] private ExtendedImage _itemIcon;
        [SerializeField] private Button _buyButton;
        [SerializeField] private Button _closeButton;

        protected override void OnPopupModelUpdate(CompositeDisposable modelBindings)
        {
            _itemNameText.text = PopupModel.ItemName;
            _descriptionText.text = PopupModel.Description;
            _itemIcon.LoadSpriteAsync(PopupModel.ItemIconPath);

            _buyButton.onClick.AsObservable()
                .Subscribe(_ => PopupModel.RequestBuy())
                .AddTo(modelBindings);

            _closeButton.onClick.AsObservable()
                .Subscribe(_ => ClosePopup())
                .AddTo(modelBindings);

            PopupModel.BuyRequested
                .Subscribe(_ => ClosePopup())
                .AddTo(modelBindings);
        }

        protected override void OnUnbind()
        {
            _itemIcon.Clear();
            base.OnUnbind();
        }

        protected override void OnDestroy()
        {
            _itemIcon.Clear();
            base.OnDestroy();
        }
    }
}
