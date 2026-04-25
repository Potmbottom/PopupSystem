using System;
using Object = UnityEngine.Object;

namespace PopupShowcase.Assets
{
    public sealed class AssetHandle<T> : IDisposable where T : Object
    {
        private Action _release;

        public AssetHandle(T asset, Action release)
        {
            Asset = asset;
            _release = release;
        }

        public T Asset { get; }

        public void Dispose()
        {
            var release = _release;
            _release = null;
            release?.Invoke();
        }
    }
}
