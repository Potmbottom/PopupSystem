using PopupShowcase.MVVM.ViewModels;
using PopupShowcase.MVVM.Popups;
using PopupShowcase.MVVM.Popups.Models;
using PopupShowcase.MVVM.Popups.Service;
using PopupShowcase.Scriptables;
using UnityEngine;
using Zenject;

namespace PopupShowcase.MVVM.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private PopupPrefabConfig _popupPrefabConfig;
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private OfferCatalogConfig _offerCatalogConfig;

        public override void InstallBindings()
        {
            BindScriptables();
            BindPopupSystem();
            BindModels();
        }

        private void BindScriptables()
        {
            Container.BindInstance(_popupPrefabConfig);
            Container.BindInstance(_gameConfig);
            Container.BindInstance(_offerCatalogConfig);
        }

        private void BindPopupSystem()
        {
            Container.BindInterfacesTo<PopupFactory>()
                .AsSingle();
            Container.BindInterfacesTo<PopupRequestService>()
                .AsSingle();
            Container.BindInterfacesAndSelfTo<BlockerModel>()
                .AsSingle();
        }

        private void BindModels()
        {
            Container.Bind<PlayerStateModel>()
                .AsSingle();

            Container.BindInterfacesTo<MenuModel>().AsSingle();
            Container.BindInterfacesTo<OffersModel>().AsSingle();
        }
    }
}
