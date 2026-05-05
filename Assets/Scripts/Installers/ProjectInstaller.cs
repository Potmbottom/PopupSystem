using PopupShowcase.Assets;
using PopupShowcase.MVVM.ViewModels;
using PopupShowcase.MVVM.Popups.Service;
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
            Container.Bind<PopupQueueService>()
                .AsSingle();

            Container.Bind<RemotePlayerStateModel>()
                .AsSingle();
        }
    }
}
