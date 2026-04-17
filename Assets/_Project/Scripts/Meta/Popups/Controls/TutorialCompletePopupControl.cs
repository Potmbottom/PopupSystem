using PopupShowcase.Core;
using PopupShowcase.PopupSystem;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PopupShowcase.Meta.Popups
{
    public class TutorialCompletePopupControl : BasePopupControl<TutorialCompletePopupData>
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
