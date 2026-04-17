using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PopupShowcase.Assets
{
    public interface IAssetProvider : IDisposable
    {
        UniTask<T> LoadAssetAsync<T>(string address, object handler = null) where T : Object;
        void ReleaseHandler(string address, object handler);
    }
}
