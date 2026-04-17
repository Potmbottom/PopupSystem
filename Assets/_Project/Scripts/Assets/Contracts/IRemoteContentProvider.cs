using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PopupShowcase.Assets
{
    public interface IRemoteContentProvider : IDisposable
    {
        UniTask<T> LoadAssetAsync<T>(
            RemoteAssetReference reference,
            object handler = null,
            CancellationToken cancellationToken = default)
            where T : Object;

        void ReleaseHandler(RemoteAssetReference reference, object handler);
    }
}
