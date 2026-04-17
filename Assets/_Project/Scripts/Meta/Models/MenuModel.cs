using System;
using R3;

namespace PopupShowcase.Meta.Models
{
    public interface IMenuModel
    {
        IOffersModel Offers { get; }
        ReadOnlyReactiveProperty<bool> IsHomeVisible { get; }
        ReadOnlyReactiveProperty<string> PlayerName { get; }
        ReadOnlyReactiveProperty<bool> IsTutorialButtonVisible { get; }
        Observable<Unit> TutorialRequested { get; }
        void RequestTutorial();
        void ShowHome(string playerName);
    }

    public class MenuModel : IMenuModel, IDisposable
    {
        public IOffersModel Offers { get; }
        public ReadOnlyReactiveProperty<bool> IsHomeVisible => _isHomeVisible;
        public ReadOnlyReactiveProperty<string> PlayerName => _playerName;
        public ReadOnlyReactiveProperty<bool> IsTutorialButtonVisible => _isTutorialButtonVisible;
        public Observable<Unit> TutorialRequested => _tutorialRequested;

        private readonly ReactiveProperty<bool> _isHomeVisible = new(false);
        private readonly ReactiveProperty<string> _playerName = new(string.Empty);
        private readonly ReactiveProperty<bool> _isTutorialButtonVisible = new(false);
        private readonly Subject<Unit> _tutorialRequested = new();
        private readonly CompositeDisposable _disposables = new();

        public MenuModel(IOffersModel offers, PlayerStateModel playerState)
        {
            Offers = offers;

            SetTutorialButtonVisible(!playerState.TutorialCompleted.CurrentValue);

            playerState.TutorialCompleted
                .Subscribe(isCompleted => SetTutorialButtonVisible(!isCompleted))
                .AddTo(_disposables);
        }

        public void ShowHome(string playerName)
        {
            _playerName.Value = playerName ?? string.Empty;
            _isHomeVisible.Value = true;
        }

        public void Dispose()
        {
            _disposables.Dispose();
            _isHomeVisible.Dispose();
            _playerName.Dispose();
            _isTutorialButtonVisible.Dispose();
            _tutorialRequested.Dispose();
        }

        private void SetTutorialButtonVisible(bool isVisible)
        {
            _isTutorialButtonVisible.Value = isVisible;
        }

        public void RequestTutorial()
        {
            _tutorialRequested.OnNext(Unit.Default);
        }
    }
}
