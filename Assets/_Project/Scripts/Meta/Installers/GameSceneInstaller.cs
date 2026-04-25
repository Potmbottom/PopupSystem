using PopupShowcase.Meta.Models;
using PopupShowcase.PopupSystem;
using UnityEngine;
using Zenject;

namespace PopupShowcase.Meta.Installers
{
    public class GameSceneInstaller : MonoInstaller
    {
        [SerializeField] private PopupPrefabConfig _popupPrefabConfig;
        [SerializeField] private GameConfig _gameConfig;
        [SerializeField] private OfferCatalogConfig _offerCatalogConfig;

        public override void InstallBindings()
        {
            BindConfigs();
            BindPopupSystem();
            BindModels();
        }

        private void BindConfigs()
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
