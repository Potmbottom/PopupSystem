using PopupShowcase.Core;
using PopupShowcase.PopupSystem;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace PopupShowcase.Meta.Popups
{
    public class DailyRewardPopupControl : BasePopupControl<DailyRewardPopupData>
    {
        [SerializeField] private ExtendedText _descriptionText;
        [SerializeField] private Button _claimButton;

        protected override void OnPopupModelUpdate(CompositeDisposable modelBindings)
        {
            _descriptionText.text = PopupModel.Description;

            _claimButton.onClick.AsObservable()
                .Subscribe(_ => ClosePopup())
                .AddTo(modelBindings);
        }
    }
}
