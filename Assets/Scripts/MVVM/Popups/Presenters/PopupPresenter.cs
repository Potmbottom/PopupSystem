using System.Collections.Generic;
using PopupShowcase.MVVM.Popups.Models;
using PopupShowcase.MVVM.Popups.Service;
using R3;
using UnityEngine;
using Zenject;

namespace PopupShowcase.MVVM.Popups.Presenters
{
    public class PopupPresenter : MonoBehaviour
    {
        [SerializeField] private Transform _popupRoot;

        private PopupQueueService _provider;
        private IPopupFactory _factory;
        private BlockerModel _blockerModel;

        private BasePopupView _currentPopup;
        private readonly Dictionary<PopupType, BasePopupView> _pool = new();
        private readonly CompositeDisposable _disposables = new();

        private PopupType _currentType;

        [Inject]
        public void SetDependency(PopupQueueService provider, IPopupFactory factory, BlockerModel blockerModel)
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

        private void OnCurrentItemChanged(QueueModel model)
        {
            if (_currentPopup != null)
                ReleaseCurrentPopup();

            if (model != null)
                ShowPopup(model);

            _blockerModel.SetVisible(_currentPopup != null);
        }

        private void ShowPopup(QueueModel item)
        {
            var model = item.Model;
            BasePopupView view;

            if (_pool.TryGetValue(model.Type, out var pooled))
            {
                _pool.Remove(model.Type);
                pooled.gameObject.SetActive(true);
                view = pooled;
            }
            else
            {
                view = _factory.Create(item);
                view.transform.SetParent(_popupRoot, false);
            }

            view.Bind(model);
            _currentPopup = view;
            _currentType = model.Type;
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
