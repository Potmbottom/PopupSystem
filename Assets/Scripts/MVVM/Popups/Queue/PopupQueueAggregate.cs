using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace PopupShowcase.MVVM.Popups
{
    public class PopupQueueAggregate<T> : ICurrentItemProvider<T>, IDisposable where T : class
    {
        public ReadOnlyReactiveProperty<T> CurrentItem => _currentItem;

        private readonly ReactiveProperty<T> _currentItem = new(default);
        private readonly IList<ICurrentItemProvider<T>> _providers;
        private readonly CompositeDisposable _subscriptions = new();
        private int _activePriority;

        public PopupQueueAggregate(IList<ICurrentItemProvider<T>> providers)
        {
            _providers = providers ?? throw new ArgumentNullException(nameof(providers));

            for (int i = 0; i < _providers.Count; i++)
            {
                var priority = i;
                _providers[i].CurrentItem
                    .Subscribe(data => OnItemChanged(priority, data))
                    .AddTo(_subscriptions);
            }
        }

        private void OnItemChanged(int priority, T data)
        {
            if (_currentItem.Value == null)
            {
                SetCurrent(priority, data);
                return;
            }

            if (priority > _activePriority) return;

            if (data != null)
            {
                SetCurrent(priority, data);
            }
            else
            {
                Debug.Assert(priority == _activePriority,
                    $"Received null for priority {priority} but active is {_activePriority}");
                FindNextActive();
            }
        }

        private void FindNextActive()
        {
            for (int i = 0; i < _providers.Count; i++)
            {
                if (_providers[i].CurrentItem.CurrentValue != null)
                {
                    SetCurrent(i, _providers[i].CurrentItem.CurrentValue);
                    return;
                }
            }

            SetCurrent(0, default);
        }

        private void SetCurrent(int priority, T data)
        {
            _activePriority = priority;
            _currentItem.Value = data;
        }

        public void Dispose()
        {
            _currentItem.Dispose();
            _subscriptions.Dispose();
        }
    }
}
