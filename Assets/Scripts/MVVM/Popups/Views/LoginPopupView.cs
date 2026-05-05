using PopupShowcase.MVVM.Popups;
using PopupShowcase.MVVM.Popups.Models;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PopupShowcase.MVVM.Popups
{
    public class LoginPopupView : BasePopupView<LoginPopupModel>
    {
        [SerializeField] private TMP_InputField _nameInput;
        [SerializeField] private Button _loginButton;

        protected override void OnPopupModelUpdate(CompositeDisposable modelBindings)
        {
            _nameInput.text = PopupModel.PlayerName.Value;

            _nameInput.onValueChanged.AsObservable()
                .Subscribe(text => PopupModel.PlayerName.Value = text)
                .AddTo(modelBindings);

            _loginButton.onClick.AsObservable()
                .Subscribe(_ => PopupModel.RequestLogin())
                .AddTo(modelBindings);

            PopupModel.LoginRequested
                .Subscribe(_ => ClosePopup())
                .AddTo(modelBindings);
        }
    }
}
