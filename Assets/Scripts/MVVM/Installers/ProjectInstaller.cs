using PopupShowcase.Assets;
using PopupShowcase.MVVM.ViewModels;
using PopupShowcase.MVVM.Popups;
using Zenject;

namespace PopupShowcase.MVVM.Installers
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
        }

        private void BindPopupQueue()
        {
            Container.Bind<PopupQueueProvider>()
                .AsSingle();

            Container.Bind<RemotePlayerStateModel>()
                .AsSingle();
        }
    }
}
