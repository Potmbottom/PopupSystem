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
        UniTask<bool> EnqueueAsync(BasePopupData data, CancellationToken cancellationToken = default);
    }

    public class PopupRequestService : IPopupRequestService
    {
        private readonly PopupQueueProvider _queueProvider;
        private readonly PopupPrefabConfig _config;
        private readonly IRemoteContentProvider _remoteContentProvider;
        private readonly HashSet<PopupType> _loadingTypes = new();

        public PopupRequestService(
            PopupQueueProvider queueProvider,
            PopupPrefabConfig config,
            IRemoteContentProvider remoteContentProvider)
        {
            _queueProvider = queueProvider;
            _config = config;
            _remoteContentProvider = remoteContentProvider;
        }

        public async UniTask<bool> EnqueueAsync(BasePopupData data, CancellationToken cancellationToken = default)
        {
            var entry = GetEntry(data.Type);
            var hasLocalPrefab = entry.Prefab != null;
            var hasRemotePrefab = entry.RemotePrefab.IsConfigured;

            if (hasLocalPrefab == hasRemotePrefab)
                throw new InvalidOperationException(
                    $"Popup type {data.Type} must have exactly one prefab source configured.");

            if (hasLocalPrefab)
            {
                _queueProvider.Enqueue(data);
                return true;
            }

            return await EnqueueRemoteAsync(data, entry, cancellationToken);
        }

        private async UniTask<bool> EnqueueRemoteAsync(
            BasePopupData data,
            PopupPrefabConfig.Entry entry,
            CancellationToken cancellationToken)
        {
            if (!_loadingTypes.Add(data.Type))
            {
                data.Dispose();
                return false;
            }

            var releaseHandle = new RemotePopupHandleRelease(
                _remoteContentProvider,
                entry.RemotePrefab);

            try
            {
                var prefab = await _remoteContentProvider.LoadAssetAsync<GameObject>(
                    entry.RemotePrefab,
                    releaseHandle.Handler,
                    cancellationToken);

                if (prefab == null)
                {
                    releaseHandle.Dispose();
                    data.Dispose();
                    return false;
                }

                data.AddDisposable(releaseHandle);
                _queueProvider.Enqueue(data, prefab);
                return true;
            }
            catch (OperationCanceledException)
            {
                releaseHandle.Dispose();
                data.Dispose();
                throw;
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    $"[PopupRequestService] Failed to load remote popup for {data.Type}. {exception.Message}");
                releaseHandle.Dispose();
                data.Dispose();
                return false;
            }
            finally
            {
                _loadingTypes.Remove(data.Type);
            }
        }

        private PopupPrefabConfig.Entry GetEntry(PopupType type)
        {
            if (_config.EntriesByType.TryGetValue(type, out var entry))
                return entry;

            throw new InvalidOperationException($"No popup config entry found for popup type: {type}");
        }

        private sealed class RemotePopupHandleRelease : IDisposable
        {
            private readonly IRemoteContentProvider _remoteContentProvider;
            private readonly RemoteAssetReference _reference;
            private bool _isDisposed;

            public RemotePopupHandleRelease(
                IRemoteContentProvider remoteContentProvider,
                RemoteAssetReference reference)
            {
                _remoteContentProvider = remoteContentProvider;
                _reference = reference;
            }

            public object Handler { get; } = new();

            public void Dispose()
            {
                if (_isDisposed)
                    return;

                _isDisposed = true;
                _remoteContentProvider.ReleaseHandler(_reference, Handler);
            }
        }
    }
}
