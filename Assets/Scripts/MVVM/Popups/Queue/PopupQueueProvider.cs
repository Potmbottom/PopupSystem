using System;
using System.Collections.Generic;
using System.Linq;
using PopupShowcase.MVVM.Popups.Models;
using R3;
using UnityEngine;

namespace PopupShowcase.MVVM.Popups
{
    public class PopupQueueProvider : IDisposable
    {
        public ReadOnlyReactiveProperty<PopupQueueItem> CurrentItem => _aggregate.CurrentItem;

        private readonly Dictionary<PopupPriority, PopupQueue<PopupQueueItem>> _queues;
        private readonly Dictionary<BasePopupModel, PopupQueueItem> _queueItems = new();
        private readonly Dictionary<BasePopupModel, IDisposable> _closeSubscriptions = new();
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

        public void Enqueue(BasePopupModel model)
        {
            Enqueue(new PopupQueueItem(model));
        }

        public void Enqueue(BasePopupModel model, GameObject loadedPrefab)
        {
            Enqueue(new PopupQueueItem(model, loadedPrefab));
        }

        private void Enqueue(PopupQueueItem item)
        {
            var model = item.Model;
            if (_queueItems.ContainsKey(model))
                throw new InvalidOperationException($"[PopupQueueProvider] Cannot enqueue the same popup instance twice");

            if (!_queues.TryGetValue(model.Priority, out var queue))
            {
                Debug.LogError($"[PopupQueueProvider] No queue for priority {model.Priority}");
                return;
            }

            _queueItems[model] = item;
            queue.Enqueue(item);
            _closeSubscriptions[model] = model.Closed.Subscribe(_ => Dequeue(model));
        }

        public void Dequeue(BasePopupModel model)
        {
            if (!_queueItems.TryGetValue(model, out var item))
            {
                Debug.LogWarning($"[PopupQueueProvider] Model {model.Type} not found in any queue");
                return;
            }

            foreach (var queue in _queues.Values)
            {
                if (!queue.Contains(item)) continue;

                queue.Dequeue(item);
                _queueItems.Remove(model);

                if (_closeSubscriptions.Remove(model, out var sub))
                    sub.Dispose();
                model.Dispose();
                return;
            }

            _queueItems.Remove(model);
            Debug.LogWarning($"[PopupQueueProvider] Model {model.Type} not found in any queue");
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
