using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace PopupShowcase.Assets
{
    public class AddressableAssetProvider : IAssetProvider
    {
        private readonly Dictionary<string, object> _cache = new();
        private readonly Dictionary<string, HashSet<object>> _handlers = new();

        public async UniTask<T> LoadAssetAsync<T>(string address, object handler = null) where T : Object
        {
            if (_cache.TryGetValue(address, out var cached))
            {
                RegisterHandler(address, handler);
                return (T)cached;
            }

            var result = await Addressables.LoadAssetAsync<T>(address).ToUniTask();
            CacheAsset(address, result);
            RegisterHandler(address, handler);
            return result;
        }

        public void ReleaseHandler(string address, object handler)
        {
            if (handler == null) return;
            if (!_handlers.TryGetValue(address, out var handlers)) return;

            handlers.Remove(handler);

            if (handlers.Count == 0 && _cache.TryGetValue(address, out var asset))
            {
                Debug.Log($"[AddressableAssetProvider] All handlers released for {address}, unloading asset");
                ReleaseInternal(address, asset);
            }
        }

        private void CacheAsset(string address, object asset)
        {
            _cache[address] = asset;
        }

        private void RegisterHandler(string address, object handler)
        {
            if (handler == null) return;

            if (!_handlers.TryGetValue(address, out var handlers))
            {
                handlers = new HashSet<object>();
                _handlers[address] = handlers;
            }

            handlers.Add(handler);
        }

        private void ReleaseInternal(string address, object asset)
        {
            Addressables.Release(asset);
            _cache.Remove(address);
            _handlers.Remove(address);
        }

        public void Dispose()
        {
            foreach (var asset in _cache.Values)
                Addressables.Release(asset);

            _cache.Clear();
            _handlers.Clear();
        }
    }
}
