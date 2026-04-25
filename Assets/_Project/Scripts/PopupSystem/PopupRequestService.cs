using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PopupShowcase.Assets;
using PopupShowcase.Meta;
using UnityEngine;

namespace PopupShowcase.PopupSystem
{
    public interface IPopupRequestService
    {
        /// <remarks>
        /// Returns false in two cases: the same popup type is already mid-load (this request is
        /// dropped to dedupe), or the addressable load failed. Throws on cancellation.
        /// </remarks>
        UniTask<bool> EnqueueAsync(
            BasePopupData data,
            CancellationToken cancellationToken = default);
    }

    public class PopupRequestService : IPopupRequestService, IDisposable
    {
        private readonly PopupQueueProvider _queueProvider;
        private readonly PopupPrefabConfig _config;
        private readonly IAssetProvider _assetProvider;
        private readonly HashSet<PopupType> _loadingTypes = new();
        private readonly Dictionary<PopupType, AssetHandle<GameObject>> _loadedHandles = new();

        public PopupRequestService(
            PopupQueueProvider queueProvider,
            PopupPrefabConfig config,
            IAssetProvider assetProvider)
        {
            _queueProvider = queueProvider;
            _config = config;
            _assetProvider = assetProvider;
        }

        public async UniTask<bool> EnqueueAsync(
            BasePopupData data,
            CancellationToken cancellationToken = default)
        {
            var entry = _config.Get(data.Type);
            var hasLocalPrefab = entry.Prefab != null;
            var hasAddress = !string.IsNullOrWhiteSpace(entry.Address);

            if (hasLocalPrefab == hasAddress)
                throw new InvalidOperationException(
                    $"Popup type {data.Type} must have exactly one prefab source configured.");

            if (hasLocalPrefab)
            {
                _queueProvider.Enqueue(data);
                return true;
            }

            if (_loadedHandles.TryGetValue(data.Type, out var cached))
            {
                _queueProvider.Enqueue(data, cached.Asset);
                return true;
            }

            return await EnqueueAddressableAsync(data, entry, cancellationToken);
        }

        private async UniTask<bool> EnqueueAddressableAsync(
            BasePopupData data,
            PopupPrefabConfig.Entry entry,
            CancellationToken cancellationToken)
        {
            if (!_loadingTypes.Add(data.Type))
            {
                data.Dispose();
                return false;
            }

            try
            {
                var handle = await _assetProvider.LoadAssetAsync<GameObject>(
                    entry.Address, cancellationToken);

                _loadedHandles[data.Type] = handle;
                _queueProvider.Enqueue(data, handle.Asset);
                return true;
            }
            catch (Exception exception)
            {
                data.Dispose();

                if (exception is OperationCanceledException)
                    throw;

                Debug.LogWarning(
                    $"[PopupRequestService] Failed to load popup for {data.Type}. {exception.Message}");
                return false;
            }
            finally
            {
                _loadingTypes.Remove(data.Type);
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
