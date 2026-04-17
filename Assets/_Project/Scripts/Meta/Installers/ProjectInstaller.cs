using PopupShowcase.Assets;
using PopupShowcase.Meta.Models;
using PopupShowcase.PopupSystem;
using Zenject;

namespace PopupShowcase.Meta.Installers
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            BindAssetServices();
            BindPopupQueue();
        }

        private void BindAssetServices()
        {
            Container.BindInterfacesTo<AddressableAssetProvider>().AsSingle();

            Container.BindInterfacesTo<AssetBundleContentProvider>().AsSingle();
        }

        private void BindPopupQueue()
        {
            Container.Bind<PopupQueueProvider>()
                .AsSingle();

            Container.Bind<RemotePlayerStateLoader>()
                .AsSingle();
        }
    }
}
