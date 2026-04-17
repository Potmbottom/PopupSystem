using System;
using UnityEngine;

namespace PopupShowcase.Assets
{
    [Serializable]
    public struct RemoteAssetReference
    {
        [SerializeField] private string _bundlePath;
        [SerializeField] private string _assetName;

        public RemoteAssetReference(string bundlePath, string assetName)
        {
            _bundlePath = bundlePath;
            _assetName = assetName;
        }

        public string BundlePath => _bundlePath;
        public string AssetName => _assetName;
        public bool IsConfigured =>
            !string.IsNullOrWhiteSpace(_bundlePath) &&
            !string.IsNullOrWhiteSpace(_assetName);

        public string CacheKey => $"{_bundlePath}::{_assetName}";

        public override string ToString()
        {
            return CacheKey;
        }
    }
}
