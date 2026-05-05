using PopupShowcase.MVVM.Popups;
using PopupShowcase.MVVM.Popups.Models;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace PopupShowcase.MVVM.Popups
{
    public class SystemInterruptPopupView : BasePopupView<SystemInterruptPopupModel>
    {
        [SerializeField] private PopupShowcase.MVVM.Common.ExtendedText _messageText;
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
