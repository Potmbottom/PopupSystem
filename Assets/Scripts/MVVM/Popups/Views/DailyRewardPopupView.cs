using PopupShowcase.MVVM.Common;
using PopupShowcase.MVVM.Popups;
using PopupShowcase.MVVM.Popups.Models;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace PopupShowcase.MVVM.Popups
{
    public class DailyRewardPopupView : BasePopupView<DailyRewardPopupModel>
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
