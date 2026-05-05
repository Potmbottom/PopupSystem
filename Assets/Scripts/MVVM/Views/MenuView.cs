using PopupShowcase.MVVM.Common;
using PopupShowcase.MVVM.ViewModels;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PopupShowcase.MVVM.Views
{
    public class MenuView : BaseView<IMenuModel>
    {
        [SerializeField] private GameObject _homeContent;
        [SerializeField] private ExtendedText _playerNameText;
        [SerializeField] private OffersShowView _offersShow;
        
        [SerializeField] private Button _tutorialButton;

        protected override void OnModelUpdate(CompositeDisposable modelBindings)
        {
            _offersShow.Bind(Model.Offers);

            _tutorialButton.onClick.AsObservable()
                .Subscribe(_ => Model.RequestTutorial())
                .AddTo(modelBindings);

            Model.IsHomeVisible
                .Subscribe(isVisible => _homeContent.SetActive(isVisible))
                .AddTo(modelBindings);

            Model.PlayerName
                .Subscribe(playerName => _playerNameText.text = playerName)
                .AddTo(modelBindings);

            Model.IsTutorialButtonVisible
                .Subscribe(isVisible => _tutorialButton.gameObject.SetActive(isVisible))
                .AddTo(modelBindings);
        }

        protected override void OnUnbind()
        {
            _offersShow?.Unbind();
            base.OnUnbind();
        }
    }
}
