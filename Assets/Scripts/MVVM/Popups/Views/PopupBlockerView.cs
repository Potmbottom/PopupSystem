using PopupShowcase.MVVM.Common;
using PopupShowcase.MVVM.Popups.Models;
using R3;
using UnityEngine;
using Zenject;

namespace PopupShowcase.MVVM.Popups.Presenters
{
    public class PopupBlockerView : BasePresenter
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        [Inject]
        public void SetDependency(IBlockerModel blockerModel)
        {
            blockerModel.IsVisible
                .Subscribe(OnVisibilityChanged)
                .AddTo(Disposables);
        }

        private void OnVisibilityChanged(bool isVisible)
        {
            if (isVisible)
                Block();
            else
                Unblock();
        }

        private void Block()
        {
            gameObject.SetActive(true);
            _canvasGroup.blocksRaycasts = true;
        }

        private void Unblock()
        {
            _canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }
    }
}
