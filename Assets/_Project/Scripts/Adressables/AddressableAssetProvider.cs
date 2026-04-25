using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace PopupShowcase.Assets
{
    public class AddressableAssetProvider : IAssetProvider
    {
        public async UniTask<AssetHandle<T>> LoadAssetAsync<T>(
            string address,
            CancellationToken cancellationToken = default) where T : Object
        {
            var asset = await Addressables.LoadAssetAsync<T>(address)
                .ToUniTask(cancellationToken: cancellationToken);

            return new AssetHandle<T>(asset, () => Addressables.Release(asset));
        }
    }
}
