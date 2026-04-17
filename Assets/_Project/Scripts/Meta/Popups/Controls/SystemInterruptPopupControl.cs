using PopupShowcase.PopupSystem;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace PopupShowcase.Meta.Popups
{
    public class SystemInterruptPopupControl : BasePopupControl<SystemInterruptPopupData>
    {
        [SerializeField] private PopupShowcase.Core.ExtendedText _messageText;
        [SerializeField] private Button _dismissButton;

        protected override void OnPopupModelUpdate(CompositeDisposable modelBindings)
        {
            _messageText.text = PopupModel.Message;

            _dismissButton.onClick.AsObservable()
                .Subscribe(_ => ClosePopup())
                .AddTo(modelBindings);
        }
    }
}
