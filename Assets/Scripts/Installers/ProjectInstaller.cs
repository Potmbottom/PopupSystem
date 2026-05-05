using PopupShowcase.Assets;
using PopupShowcase.MVVM.Popups.Models;
using PopupShowcase.MVVM.ViewModels;
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
            Container.Bind<PopupQueueModel>()
                .AsSingle();

            Container.Bind<RemotePlayerStateModel>()
                .AsSingle();
        }
    }
}
