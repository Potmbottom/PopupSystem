using System.Collections.Generic;
using R3;
using UnityEngine;
using Zenject;

namespace PopupShowcase.PopupSystem
{
    public class PopupPresenter : MonoBehaviour
    {
        [SerializeField] private Transform _popupRoot;

        private PopupQueueProvider _provider;
        private IPopupFactory _factory;
        private BlockerModel _blockerModel;

        private BasePopupControl _currentPopup;
        private readonly Dictionary<PopupType, BasePopupControl> _pool = new();
        private readonly CompositeDisposable _disposables = new();

        private PopupType _currentType;

        [Inject]
        public void SetDependency(PopupQueueProvider provider, IPopupFactory factory, BlockerModel blockerModel)
        {
            _provider = provider;
            _factory = factory;
            _blockerModel = blockerModel;
        }

        private void Awake()
        {
            _provider.CurrentItem
                .Subscribe(OnCurrentItemChanged)
                .AddTo(_disposables);
        }

        private void OnCurrentItemChanged(PopupQueueItem item)
        {
            if (_currentPopup != null)
                ReleaseCurrentPopup();

            if (item != null)
                ShowPopup(item);

            _blockerModel.SetVisible(_currentPopup != null);
        }

        private void ShowPopup(PopupQueueItem item)
        {
            var data = item.Data;
            BasePopupControl control;

            if (_pool.TryGetValue(data.Type, out var pooled))
            {
                _pool.Remove(data.Type);
                pooled.gameObject.SetActive(true);
                control = pooled;
            }
            else
            {
                control = _factory.Create(item);
                control.transform.SetParent(_popupRoot, false);
            }

            control.Bind(data);
            _currentPopup = control;
            _currentType = data.Type;
        }

        private void ReleaseCurrentPopup()
        {
            _currentPopup.Unbind();
            _currentPopup.gameObject.SetActive(false);
            _pool[_currentType] = _currentPopup;
            _currentPopup = null;
        }

        private void OnDestroy()
        {
            _disposables.Dispose();

            if (_currentPopup != null)
                Destroy(_currentPopup.gameObject);

            foreach (var pooled in _pool.Values)
            {
                if (pooled != null)
                    Destroy(pooled.gameObject);
            }
            _pool.Clear();
        }
    }
}
