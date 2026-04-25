using System.Threading;
using Cysharp.Threading.Tasks;
using Object = UnityEngine.Object;

namespace PopupShowcase.Assets
{
    public interface IAssetProvider
    {
        UniTask<AssetHandle<T>> LoadAssetAsync<T>(
            string address,
            CancellationToken cancellationToken = default) where T : Object;
    }
}
