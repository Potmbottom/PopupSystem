using System;
using PopupShowcase.Meta;
using UnityEngine;
using Zenject;

namespace PopupShowcase.PopupSystem
{
    public interface IPopupFactory
    {
        BasePopupControl CreateLocal(PopupType type);
        BasePopupControl CreateLoaded(GameObject prefab);
        bool ShouldPool(PopupType type);
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

        public BasePopupControl CreateLocal(PopupType type)
        {
            var prefab = GetLocalPrefab(type);
            return CreateLoaded(prefab);
        }

        public BasePopupControl CreateLoaded(GameObject prefab)
        {
            return _container.InstantiatePrefabForComponent<BasePopupControl>(prefab);
        }

        public bool ShouldPool(PopupType type)
        {
            var entry = GetEntry(type);
            return entry.Prefab != null && !entry.RemotePrefab.IsConfigured;
        }

        private GameObject GetLocalPrefab(PopupType type)
        {
            var entry = GetEntry(type);
            var hasLocalPrefab = entry.Prefab != null;
            var hasRemotePrefab = entry.RemotePrefab.IsConfigured;

            if (hasLocalPrefab && !hasRemotePrefab)
                return entry.Prefab;

            if (hasRemotePrefab)
                throw new InvalidOperationException(
                    $"Popup type {type} requires a runtime-loaded prefab before Create is called.");

            throw new InvalidOperationException($"Popup prefab is not configured for type: {type}");
        }

        private PopupPrefabConfig.Entry GetEntry(PopupType type)
        {
            if (_config.EntriesByType.TryGetValue(type, out var entry))
                return entry;

            throw new InvalidOperationException($"No popup config entry found for popup type: {type}");
        }
    }
}
