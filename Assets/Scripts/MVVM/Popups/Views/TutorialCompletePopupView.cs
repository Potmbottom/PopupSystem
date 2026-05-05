using PopupShowcase.MVVM.Common;
using PopupShowcase.MVVM.Popups;
using PopupShowcase.MVVM.Popups.Models;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PopupShowcase.MVVM.Popups
{
    public class TutorialCompletePopupView : BasePopupView<TutorialCompletePopupModel>
    {
        [SerializeField] private TMP_Text _titleText;
        [SerializeField] private TMP_Text _descriptionText;
        [SerializeField] private Button _submitButton;

        protected override void OnPopupModelUpdate(CompositeDisposable modelBindings)
        {
            if (_titleText != null)
                _titleText.text = PopupModel.Title;

            if (_descriptionText != null)
                _descriptionText.text = PopupModel.Description;

            _submitButton.onClick.AsObservable()
                .Subscribe(_ => ClosePopup())
                .AddTo(modelBindings);
        }
    }
}
