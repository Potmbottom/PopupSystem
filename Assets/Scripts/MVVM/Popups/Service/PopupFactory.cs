using System;
using PopupShowcase.MVVM.Popups.Models;
using PopupShowcase.Scriptables;
using UnityEngine;
using Zenject;

namespace PopupShowcase.MVVM.Popups.Service
{
    public interface IPopupFactory
    {
        BasePopupView Create(QueueModel model);
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

        public BasePopupView Create(QueueModel model)
        {
            var prefab = model.LoadedPrefab ?? _config.Get(model.Model.Type)?.Prefab;
            if (prefab == null)
                throw new InvalidOperationException(
                    $"No prefab available for popup type {model.Model.Type}.");

            return _container.InstantiatePrefabForComponent<BasePopupView>(prefab);
        }
    }
}
