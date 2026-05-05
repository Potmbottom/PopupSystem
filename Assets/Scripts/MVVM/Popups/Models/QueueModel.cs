using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

namespace PopupShowcase.MVVM.Popups
{
    public interface ICurrentItemProvider<T>
    {
        ReadOnlyReactiveProperty<T> CurrentItem { get; }
    }

    public class QueueModel<T> : ICurrentItemProvider<T>, IDisposable where T : class
    {
        public ReadOnlyReactiveProperty<T> CurrentItem => _currentItem;
        
        private readonly ReactiveProperty<T> _currentItem = new(default);
        private readonly LinkedList<T> _queue = new();
        private readonly Subject<T> _dequeued = new();
        private readonly string _name;

        public QueueModel(string name)
        {
            _name = name;
        }

        public void Enqueue(T data)
        {
            if (_queue.Contains(data))
                throw new InvalidOperationException($"[PopupQueue:{_name}] Cannot enqueue the same instance twice");

            _queue.AddLast(data);
            Debug.Log($"[PopupQueue:{_name}] Enqueue {data}");

            if (_currentItem.Value == null)
                _currentItem.Value = data;
        }

        public void Dequeue(T data)
        {
            if (!_queue.Remove(data)) return;

            if (ReferenceEquals(_currentItem.Value, data))
                _currentItem.Value = _queue.FirstOrDefault();

            Debug.Log($"[PopupQueue:{_name}] Dequeue {data}");
            _dequeued.OnNext(data);
        }

        public bool Contains(T data) => _queue.Contains(data);

        public void Dispose()
        {
            _currentItem.Dispose();
            _dequeued.Dispose();
        }
    }
}
