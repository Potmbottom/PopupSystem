using System.Collections.Generic;
using R3;
using UnityEngine;
using Zenject;

namespace PopupShowcase.PopupSystem
{
    public class PopupContainer : MonoBehaviour
    {
        [SerializeField] private Transform _popupRoot;
        [SerializeField] private PopupBlocker _blocker;

        private PopupQueueProvider _provider;
        private IPopupFactory _factory;

        private BasePopupControl _currentPopup;
        private PopupType _currentType;
        private readonly Dictionary<PopupType, BasePopupControl> _pool = new();
        private readonly CompositeDisposable _disposables = new();
        private bool _isBlockerVisible;

        [Inject]
        public void SetDependency(PopupQueueProvider provider, IPopupFactory factory)
        {
            _provider = provider;
            _factory = factory;
        }

        private void Awake()
        {
            RefreshBlocker();

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

            RefreshBlocker();
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
                control = item.LoadedPrefab != null
                    ? _factory.CreateLoaded(item.LoadedPrefab)
                    : _factory.CreateLocal(data.Type);
                control.transform.SetParent(_popupRoot, false);
            }

            control.Bind(data);
            _currentPopup = control;
            _currentType = data.Type;
        }

        private void ReleaseCurrentPopup()
        {
            _currentPopup.Unbind();

            if (_factory.ShouldPool(_currentType))
            {
                _currentPopup.gameObject.SetActive(false);
                _pool[_currentType] = _currentPopup;
            }
            else
            {
                Destroy(_currentPopup.gameObject);
            }

            _currentPopup = null;
        }

        private void RefreshBlocker()
        {
            var shouldBlock = _currentPopup != null;
            if (shouldBlock == _isBlockerVisible)
                return;

            _isBlockerVisible = shouldBlock;

            if (!shouldBlock)
            {
                if (_blocker.gameObject.activeSelf)
                    _blocker.Unblock();
                return;
            }

            _blocker.Block();
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
