using PopupShowcase.Core;
using R3;
using UnityEngine;
using Zenject;

namespace PopupShowcase.PopupSystem
{
    public class PopupBlocker : BaseTweenAnimation
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public void SetDependency(IBlockerModel blockerModel)
        {
            blockerModel.IsVisible
                .Subscribe(OnVisibilityChanged)
                .AddTo(_disposables);
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

        private void OnDestroy()
        {
            _disposables.Dispose();
        }
    }
}
