using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using PopupShowcase.MVVM.Common;
using PopupShowcase.MVVM.Views;
using PopupShowcase.MVVM.ViewModels;
using PopupShowcase.MVVM.Popups;
using PopupShowcase.MVVM.Popups.Models;
using PopupShowcase.MVVM.Popups.Service;
using PopupShowcase.Scriptables;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PopupShowcase.MVVM.Presenters
{
    public class GamePresenter : BasePresenter
    {
        private const string RemoteLoadFailureMessage = "Failed to load remote content. Please try again.";

        [SerializeField] private CanvasGroupFadeAnimation _loadingOverlayAnimation;
        [SerializeField] private Button _debugSystemErrorButton;
        [SerializeField] private Button _debugDailyRewardButton;
        [SerializeField] private MenuView _menuView;

        private PopupQueueService _popupQueue;
        private IPopupRequestService _popupRequestService;
        private PlayerStateModel _playerState;
        private IOffersModel _offersModel;
        private IMenuModel _menuModel;
        private GameConfig _gameConfig;
        private readonly CancellationTokenSource _cts = new();

        private string _playerName;
        private bool _isFirstLoadingState = true;
        private LoadingPopupModel _loadingModel;

        [Inject]
        public void SetDependency(
            PopupQueueService popupQueue,
            IPopupRequestService popupRequestService,
            PlayerStateModel playerState,
            IOffersModel offersModel,
            IMenuModel menuModel,
            GameConfig gameConfig)
        {
            _popupQueue = popupQueue;
            _popupRequestService = popupRequestService;
            _playerState = playerState;
            _offersModel = offersModel;
            _menuModel = menuModel;
            _gameConfig = gameConfig;
        }

        private void Awake()
        {
            _loadingOverlayAnimation.HideInstant();
            _menuView.Bind(_menuModel);

            _menuModel.TutorialRequested
                .Subscribe(_ => HandleTutorialClicked())
                .AddTo(Disposables);

            _debugSystemErrorButton.onClick.AsObservable()
                .Subscribe(_ => HandleDebugSystemError())
                .AddTo(Disposables);

            _debugDailyRewardButton.onClick.AsObservable()
                .Subscribe(_ => HandleDebugDailyReward())
                .AddTo(Disposables);

            _offersModel.OfferSelected
                .Subscribe(HandleOfferClicked)
                .AddTo(Disposables);

            RunStartupFlowAsync().Forget();
        }

        private async UniTaskVoid RunStartupFlowAsync()
        {
            try
            {
                ShowLoading();
                await UniTask.Delay(_gameConfig.StartupLoadingDelayMs, cancellationToken: _cts.Token);
                await _playerState.InitializeAsync(_cts.Token);
                HideLoading();

                var loginModel = new LoginPopupModel();
                loginModel.AddDisposable(loginModel.LoginRequested.Subscribe(name => _playerName = name));
                _popupQueue.Enqueue(loginModel);

                var rewardModel = new DailyRewardPopupModel(
                    "Welcome back! Here's your daily bonus.");
                rewardModel.AddDisposable(rewardModel.Closed.Subscribe(_ => _menuModel.ShowHome(_playerName)));
                _popupQueue.Enqueue(rewardModel);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void HandleTutorialClicked()
        {
            _loadingModel = new LoadingPopupModel();
            _popupQueue.Enqueue(_loadingModel);
            LoadTutorialPopupAsync().Forget();
        }

        private async UniTaskVoid LoadTutorialPopupAsync()
        {
            var loadingModel = _loadingModel;
            if (loadingModel == null)
                return;

            try
            {
                var tutorialModel = CreateTutorialPopupModel();
                await TryEnqueuePopupAsync(tutorialModel, _cts.Token);
            }
            catch (OperationCanceledException) { return; }
            catch (Exception e)
            {
                Debug.LogException(e);
                ShowRemoteLoadFailure();
            }
            finally
            {
                await UniTask.Delay(3000);
                _popupQueue.Dequeue(loadingModel);
                if (ReferenceEquals(_loadingModel, loadingModel))
                    _loadingModel = null;
            }
        }

        private TutorialCompletePopupModel CreateTutorialPopupModel()
        {
            var model = new TutorialCompletePopupModel(
                "Tutorial Complete!",
                "Congratulations! You've unlocked new content.");
            model.AddDisposable(model.Activated.Subscribe(_ => _playerState.MarkTutorialComplete()));
            return model;
        }

        private void HandleOfferClicked(string offerId)
        {
            HandleOfferClickedAsync(offerId).Forget();
        }

        private async UniTaskVoid HandleOfferClickedAsync(string offerId)
        {
            if (!_offersModel.TryGetOffer(offerId, out var offer))
                return;

            var model = new OfferPopupModel(
                offer.PopupTitle,
                offer.PopupDescription,
                offer.IconPath);

            model.AddDisposable(model.BuyRequested.Subscribe(_ => HandleOfferBought(offerId)));
            await TryEnqueuePopupAsync(model, _cts.Token);
        }

        private void HandleOfferBought(string offerId)
        {
            _playerState.MarkOfferPurchased(offerId);
        }

        private void HandleDebugSystemError()
        {
            TryEnqueuePopupAsync(
                new SystemInterruptPopupModel("An unexpected error occurred. Please try again."),
                _cts.Token).Forget();
        }

        private void HandleDebugDailyReward()
        {
            TryEnqueuePopupAsync(
                new DailyRewardPopupModel("Bonus coins for you!"),
                _cts.Token).Forget();
        }

        private async UniTask<bool> TryEnqueuePopupAsync(
            BasePopupModel model,
            CancellationToken cancellationToken)
        {
            try
            {
                var isEnqueued = await _popupRequestService.EnqueueAsync(model, cancellationToken);
                if (!isEnqueued)
                    ShowRemoteLoadFailure();

                return isEnqueued;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                ShowRemoteLoadFailure();
                return false;
            }
        }

        private void ShowRemoteLoadFailure()
        {
            _popupQueue.Enqueue(new SystemInterruptPopupModel(RemoteLoadFailureMessage));
        }

        private void ShowLoading()
        {
            ApplyLoadingState(true, false);
        }

        private void HideLoading()
        {
            ApplyLoadingState(false, !_isFirstLoadingState);
        }

        private void ApplyLoadingState(bool isVisible, bool animate)
        {
            _isFirstLoadingState = false;

            if (isVisible)
            {
                _loadingOverlayAnimation.ShowInstant();
                return;
            }

            if (!animate)
            {
                _loadingOverlayAnimation.HideInstant();
                return;
            }

            _loadingOverlayAnimation.FadeOutAndDisable();
        }

        public override void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
            base.Dispose();
        }
    }
}
