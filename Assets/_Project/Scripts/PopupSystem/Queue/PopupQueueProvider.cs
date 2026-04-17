using System;
using System.Collections.Generic;
using System.Linq;
using R3;
using UnityEngine;

namespace PopupShowcase.PopupSystem
{
    public class PopupQueueProvider : IDisposable
    {
        public ReadOnlyReactiveProperty<PopupQueueItem> CurrentItem => _aggregate.CurrentItem;

        private readonly Dictionary<PopupPriority, PopupQueue<PopupQueueItem>> _queues;
        private readonly Dictionary<BasePopupData, PopupQueueItem> _queueItems = new();
        private readonly Dictionary<BasePopupData, IDisposable> _closeSubscriptions = new();
        private readonly PopupQueueAggregate<PopupQueueItem> _aggregate;

        public PopupQueueProvider()
        {
            _queues = new Dictionary<PopupPriority, PopupQueue<PopupQueueItem>>();
            foreach (PopupPriority priority in Enum.GetValues(typeof(PopupPriority)))
                _queues[priority] = new PopupQueue<PopupQueueItem>(priority.ToString());

            var orderedProviders = _queues
                .OrderBy(kv => (int)kv.Key)
                .Select(kv => (ICurrentItemProvider<PopupQueueItem>)kv.Value)
                .ToList();

            _aggregate = new PopupQueueAggregate<PopupQueueItem>(orderedProviders);
        }

        public void Enqueue(BasePopupData data)
        {
            Enqueue(new PopupQueueItem(data));
        }

        public void Enqueue(BasePopupData data, GameObject loadedPrefab)
        {
            Enqueue(new PopupQueueItem(data, loadedPrefab));
        }

        private void Enqueue(PopupQueueItem item)
        {
            var data = item.Data;
            if (_queueItems.ContainsKey(data))
                throw new InvalidOperationException($"[PopupQueueProvider] Cannot enqueue the same popup instance twice");

            if (!_queues.TryGetValue(data.Priority, out var queue))
            {
                Debug.LogError($"[PopupQueueProvider] No queue for priority {data.Priority}");
                return;
            }

            _queueItems[data] = item;
            queue.Enqueue(item);
            _closeSubscriptions[data] = data.Closed.Subscribe(_ => Dequeue(data));
        }

        public void Dequeue(BasePopupData data)
        {
            if (!_queueItems.TryGetValue(data, out var item))
            {
                Debug.LogWarning($"[PopupQueueProvider] Data {data.Type} not found in any queue");
                return;
            }

            foreach (var queue in _queues.Values)
            {
                if (!queue.Contains(item)) continue;

                queue.Dequeue(item);
                _queueItems.Remove(data);

                if (_closeSubscriptions.Remove(data, out var sub))
                    sub.Dispose();
                data.Dispose();
                return;
            }

            _queueItems.Remove(data);
            Debug.LogWarning($"[PopupQueueProvider] Data {data.Type} not found in any queue");
        }

        public void Dispose()
        {
            _aggregate.Dispose();

            foreach (var sub in _closeSubscriptions.Values)
                sub.Dispose();
            _closeSubscriptions.Clear();
            _queueItems.Clear();

            foreach (var queue in _queues.Values)
                queue.Dispose();
        }
    }
}
