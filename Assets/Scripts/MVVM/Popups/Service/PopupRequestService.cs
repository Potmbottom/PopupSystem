using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PopupShowcase.Assets;
using PopupShowcase.MVVM.Popups.Models;
using PopupShowcase.Scriptables;
using UnityEngine;

namespace PopupShowcase.MVVM.Popups.Service
{
    public interface IPopupRequestService
    {
        /// <remarks>
        /// Returns false in two cases: the same popup type is already mid-load (this request is
        /// dropped to dedupe), or the addressable load failed. Throws on cancellation.
        /// </remarks>
        UniTask<bool> EnqueueAsync(
            BasePopupModel model,
            CancellationToken cancellationToken = default);
    }

    public class PopupRequestService : IPopupRequestService, IDisposable
    {
        private readonly PopupQueueService _queueProvider;
        private readonly PopupPrefabConfig _config;
        private readonly IAssetProvider _assetProvider;
        private readonly HashSet<PopupType> _loadingTypes = new();
        private readonly Dictionary<PopupType, AssetHandle<GameObject>> _loadedHandles = new();

        public PopupRequestService(
            PopupQueueService queueProvider,
            PopupPrefabConfig config,
            IAssetProvider assetProvider)
        {
            _queueProvider = queueProvider;
            _config = config;
            _assetProvider = assetProvider;
        }

        public async UniTask<bool> EnqueueAsync(
            BasePopupModel model,
            CancellationToken cancellationToken = default)
        {
            var entry = _config.Get(model.Type);
            var hasLocalPrefab = entry.Prefab != null;
            var hasAddress = !string.IsNullOrWhiteSpace(entry.Address);

            if (hasLocalPrefab == hasAddress)
                throw new InvalidOperationException(
                    $"Popup type {model.Type} must have exactly one prefab source configured.");

            if (hasLocalPrefab)
            {
                _queueProvider.Enqueue(model);
                return true;
            }

            if (_loadedHandles.TryGetValue(model.Type, out var cached))
            {
                _queueProvider.Enqueue(model, cached.Asset);
                return true;
            }

            return await EnqueueAddressableAsync(model, entry, cancellationToken);
        }

        private async UniTask<bool> EnqueueAddressableAsync(
            BasePopupModel model,
            PopupPrefabConfig.Entry entry,
            CancellationToken cancellationToken)
        {
            if (!_loadingTypes.Add(model.Type))
            {
                model.Dispose();
                return false;
            }

            try
            {
                var handle = await _assetProvider.LoadAssetAsync<GameObject>(
                    entry.Address, cancellationToken);

                _loadedHandles[model.Type] = handle;
                _queueProvider.Enqueue(model, handle.Asset);
                return true;
            }
            catch (Exception exception)
            {
                model.Dispose();

                if (exception is OperationCanceledException)
                    throw;

                Debug.LogWarning(
                    $"[PopupRequestService] Failed to load popup for {model.Type}. {exception.Message}");
                return false;
            }
            finally
            {
                _loadingTypes.Remove(model.Type);
            }
        }

        public void Dispose()
        {
            foreach (var handle in _loadedHandles.Values)
                handle.Dispose();
            _loadedHandles.Clear();
        }
    }
}
