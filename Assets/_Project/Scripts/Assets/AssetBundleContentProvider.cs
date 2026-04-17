using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

namespace PopupShowcase.Assets
{
    public class AssetBundleContentProvider : IRemoteContentProvider
    {
        private sealed class BundleEntry
        {
            public AssetBundle Bundle;
            public Dictionary<string, AssetEntry> Assets { get; } = new();
        }

        private sealed class AssetEntry
        {
            public Object Asset;
            public HashSet<object> Handlers { get; } = new();
        }

        private readonly Dictionary<string, BundleEntry> _bundles = new();

        public async UniTask<T> LoadAssetAsync<T>(
            RemoteAssetReference reference,
            object handler = null,
            CancellationToken cancellationToken = default)
            where T : Object
        {
            if (!reference.IsConfigured)
                throw new InvalidOperationException("Remote asset reference is not configured.");

            var bundleEntry = await LoadBundleAsync(reference.BundlePath, cancellationToken);

            if (bundleEntry.Assets.TryGetValue(reference.AssetName, out var cachedEntry))
            {
                RegisterHandler(cachedEntry, handler);
                return (T)cachedEntry.Asset;
            }

            var loadRequest = bundleEntry.Bundle.LoadAssetAsync<T>(reference.AssetName);
            await UniTask.WaitUntil(() => loadRequest.isDone, cancellationToken: cancellationToken);
            var asset = loadRequest.asset as T;

            if (asset == null)
                throw new InvalidOperationException(
                    $"Asset '{reference.AssetName}' was not found in bundle '{reference.BundlePath}'.");

            var newEntry = new AssetEntry
            {
                Asset = asset
            };

            RegisterHandler(newEntry, handler);
            bundleEntry.Assets[reference.AssetName] = newEntry;
            return asset;
        }

        public void ReleaseHandler(RemoteAssetReference reference, object handler)
        {
            if (!reference.IsConfigured || handler == null)
                return;

            if (!_bundles.TryGetValue(reference.BundlePath, out var bundleEntry))
                return;

            if (!bundleEntry.Assets.TryGetValue(reference.AssetName, out var assetEntry))
                return;

            assetEntry.Handlers.Remove(handler);
            if (assetEntry.Handlers.Count > 0)
                return;

            bundleEntry.Assets.Remove(reference.AssetName);

            if (bundleEntry.Assets.Count > 0)
                return;

            bundleEntry.Bundle.Unload(false);
            _bundles.Remove(reference.BundlePath);
            Debug.Log($"[AssetBundleContentProvider] Unloaded bundle '{reference.BundlePath}'.");
        }

        public void Dispose()
        {
            foreach (var bundle in _bundles.Values)
                bundle.Bundle.Unload(false);

            _bundles.Clear();
        }

        private async UniTask<BundleEntry> LoadBundleAsync(string bundlePath, CancellationToken cancellationToken)
        {
            if (_bundles.TryGetValue(bundlePath, out var cachedBundle))
                return cachedBundle;

            var resolvedUri = ResolveUri(bundlePath);
            using var request = UnityWebRequestAssetBundle.GetAssetBundle(resolvedUri);
            request.SendWebRequest();
            await UniTask.WaitUntil(() => request.isDone, cancellationToken: cancellationToken);

            if (request.result != UnityWebRequest.Result.Success)
                throw new InvalidOperationException(
                    $"Failed to download AssetBundle from '{resolvedUri}'. {request.error}");

            var bundle = DownloadHandlerAssetBundle.GetContent(request);

            if (bundle == null)
                throw new InvalidOperationException($"Failed to load AssetBundle from '{resolvedUri}'.");

            var entry = new BundleEntry
            {
                Bundle = bundle
            };

            _bundles[bundlePath] = entry;
            return entry;
        }

        private static void RegisterHandler(AssetEntry entry, object handler)
        {
            if (handler == null)
                return;

            entry.Handlers.Add(handler);
        }

        private static string ResolveUri(string bundlePath)
        {
            if (Uri.TryCreate(bundlePath, UriKind.Absolute, out var absoluteUri))
                return absoluteUri.AbsoluteUri;

            var fullPath = Path.Combine(Application.streamingAssetsPath, bundlePath);
            return new Uri(fullPath).AbsoluteUri;
        }
    }
}
