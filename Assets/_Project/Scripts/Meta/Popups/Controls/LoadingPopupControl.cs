using System.Text;
using PopupShowcase.Core;
using PopupShowcase.PopupSystem;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace PopupShowcase.Meta.Popups
{
    public class LoadingPopupControl : BasePopupControl<LoadingPopupData>
    {
        [SerializeField] private Image _progressBar;
        [SerializeField] private ExtendedText _progressText;

        private readonly StringBuilder _progressTextBuilder = new(4);

        protected override void OnPopupModelUpdate(CompositeDisposable modelBindings)
        {
            _progressBar.fillAmount = 0f;
            UpdateProgressText(0f);

            PopupModel.Progress
                .Subscribe(value =>
                {
                    _progressBar.fillAmount = value;
                    UpdateProgressText(value);
                })
                .AddTo(modelBindings);
        }

        private void UpdateProgressText(float value)
        {
            _progressTextBuilder.Clear();
            _progressTextBuilder.Append(Mathf.RoundToInt(value * 100f));
            _progressTextBuilder.Append('%');
            _progressText.SetText(_progressTextBuilder);
        }
    }
}
