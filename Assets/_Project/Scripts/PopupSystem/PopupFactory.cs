using System;
using PopupShowcase.Meta;
using UnityEngine;
using Zenject;

namespace PopupShowcase.PopupSystem
{
    public interface IPopupFactory
    {
        BasePopupControl Create(PopupQueueItem item);
    }

    public class PopupFactory : IPopupFactory
    {
        private readonly DiContainer _container;
        private readonly PopupPrefabConfig _config;

        public PopupFactory(
            DiContainer container,
            PopupPrefabConfig config)
        {
            _container = container;
            _config = config;
        }

        public BasePopupControl Create(PopupQueueItem item)
        {
            var prefab = item.LoadedPrefab ?? _config.Get(item.Data.Type)?.Prefab;
            if (prefab == null)
                throw new InvalidOperationException(
                    $"No prefab available for popup type {item.Data.Type}.");

            return _container.InstantiatePrefabForComponent<BasePopupControl>(prefab);
        }
    }
}
