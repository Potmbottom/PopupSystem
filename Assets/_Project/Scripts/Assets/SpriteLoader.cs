using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PopupShowcase.Meta;
using UnityEngine;

namespace PopupShowcase.Assets
{
    public interface ISpriteLoader
    {
        UniTask<Sprite> LoadAsync(
            string spritePath,
            object handler = null,
            CancellationToken cancellationToken = default);

        void Release(string spritePath, object handler);
    }

    public class SpriteLoader : ISpriteLoader
    {
        private const string BundlePrefix = "bundle://";

        private readonly IAssetProvider _assetProvider;
        private readonly IRemoteContentProvider _remoteContentProvider;
        private readonly GameConfig _gameConfig;
        private readonly Dictionary<string, Dictionary<object, string>> _loadedPathsByRequest = new();

        public SpriteLoader(
            IAssetProvider assetProvider,
            IRemoteContentProvider remoteContentProvider,
            GameConfig gameConfig)
        {
            _assetProvider = assetProvider;
            _remoteContentProvider = remoteContentProvider;
            _gameConfig = gameConfig;
        }

        public async UniTask<Sprite> LoadAsync(
            string spritePath,
            object handler = null,
            CancellationToken cancellationToken = default)
        {
            var requestedPath = spritePath ?? string.Empty;
            var primarySprite = await TryLoadInternalAsync(requestedPath, handler, cancellationToken);

            if (primarySprite != null)
            {
                TrackLoadedPath(requestedPath, handler, requestedPath);
                return primarySprite;
            }

            var fallbackPath = _gameConfig.DefaultSpritePath;
            if (string.IsNullOrWhiteSpace(fallbackPath) ||
                string.Equals(requestedPath, fallbackPath, StringComparison.Ordinal))
            {
                return null;
            }

            var fallbackSprite = await TryLoadInternalAsync(fallbackPath, handler, cancellationToken);
            if (fallbackSprite != null)
            {
                TrackLoadedPath(requestedPath, handler, fallbackPath);
                return fallbackSprite;
            }

            return null;
        }

        public void Release(string spritePath, object handler)
        {
            if (handler == null)
                return;

            var requestedPath = spritePath ?? string.Empty;
            var loadedPath = PopLoadedPath(requestedPath, handler);
            if (string.IsNullOrWhiteSpace(loadedPath))
                loadedPath = requestedPath;

            if (string.IsNullOrWhiteSpace(loadedPath))
                return;

            ReleaseInternal(loadedPath, handler);
        }

        private async UniTask<Sprite> TryLoadInternalAsync(
            string spritePath,
            object handler,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(spritePath))
                return null;

            try
            {
                if (TryParseBundlePath(spritePath, out var remoteReference))
                {
                    return await _remoteContentProvider.LoadAssetAsync<Sprite>(
                        remoteReference,
                        handler,
                        cancellationToken);
                }

                return await _assetProvider.LoadAssetAsync<Sprite>(spritePath, handler);
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    $"[SpriteLoader] Failed to load sprite '{spritePath}'. {exception.Message}");
                return null;
            }
        }

        private void ReleaseInternal(string spritePath, object handler)
        {
            if (TryParseBundlePath(spritePath, out var remoteReference))
            {
                _remoteContentProvider.ReleaseHandler(remoteReference, handler);
                return;
            }

            _assetProvider.ReleaseHandler(spritePath, handler);
        }

        private void TrackLoadedPath(string requestedPath, object handler, string loadedPath)
        {
            if (handler == null || string.IsNullOrWhiteSpace(loadedPath))
                return;

            if (!_loadedPathsByRequest.TryGetValue(requestedPath, out var handlers))
            {
                handlers = new Dictionary<object, string>();
                _loadedPathsByRequest[requestedPath] = handlers;
            }

            if (handlers.TryGetValue(handler, out var previousPath) &&
                !string.Equals(previousPath, loadedPath, StringComparison.Ordinal))
            {
                ReleaseInternal(previousPath, handler);
            }

            handlers[handler] = loadedPath;
        }

        private string PopLoadedPath(string requestedPath, object handler)
        {
            if (!_loadedPathsByRequest.TryGetValue(requestedPath, out var handlers))
                return null;

            if (!handlers.TryGetValue(handler, out var loadedPath))
                return null;

            handlers.Remove(handler);
            if (handlers.Count == 0)
                _loadedPathsByRequest.Remove(requestedPath);

            return loadedPath;
        }

        private static bool TryParseBundlePath(string spritePath, out RemoteAssetReference reference)
        {
            reference = default;

            if (string.IsNullOrWhiteSpace(spritePath) ||
                !spritePath.StartsWith(BundlePrefix, StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var encodedPath = spritePath.Substring(BundlePrefix.Length);
            var separatorIndex = encodedPath.LastIndexOf('#');
            if (separatorIndex <= 0 || separatorIndex >= encodedPath.Length - 1)
                return false;

            var bundlePath = encodedPath.Substring(0, separatorIndex);
            var assetName = encodedPath.Substring(separatorIndex + 1);

            reference = new RemoteAssetReference(bundlePath, assetName);
            return reference.IsConfigured;
        }
    }
}
