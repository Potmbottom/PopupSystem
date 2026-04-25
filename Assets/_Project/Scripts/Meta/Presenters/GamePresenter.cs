using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using PopupShowcase.Core;
using PopupShowcase.Meta;
using PopupShowcase.Meta.Controls;
using PopupShowcase.Meta.Models;
using PopupShowcase.Meta.Popups;
using PopupShowcase.PopupSystem;
using R3;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PopupShowcase.Meta.Presenters
{
    public class GamePresenter : BasePresenter
    {
        private const string RemoteLoadFailureMessage = "Failed to load remote content. Please try again.";

        [SerializeField] private CanvasGroupFadeAnimation _loadingOverlayAnimation;
        [SerializeField] private Button _debugSystemErrorButton;
        [SerializeField] private Button _debugDailyRewardButton;
        [SerializeField] private MenuControl _menuControl;

        private PopupQueueProvider _popupQueue;
        private IPopupRequestService _popupRequestService;
        private PlayerStateModel _playerState;
        private IOffersModel _offersModel;
        private IMenuModel _menuModel;
        private GameConfig _gameConfig;
        private readonly CancellationTokenSource _cts = new();

        private string _playerName;
        private bool _isFirstLoadingState = true;
        private LoadingPopupData _loadingData;

        [Inject]
        public void SetDependency(
            PopupQueueProvider popupQueue,
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
            _menuControl.Bind(_menuModel);

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

                var loginData = new LoginPopupData();
                loginData.AddDisposable(loginData.LoginRequested.Subscribe(name => _playerName = name));
                _popupQueue.Enqueue(loginData);

                var rewardData = new DailyRewardPopupData(
                    "Welcome back! Here's your daily bonus.");
                rewardData.AddDisposable(rewardData.Closed.Subscribe(_ => _menuModel.ShowHome(_playerName)));
                _popupQueue.Enqueue(rewardData);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private void HandleTutorialClicked()
        {
            _loadingData = new LoadingPopupData();
            _popupQueue.Enqueue(_loadingData);
            LoadTutorialPopupAsync().Forget();
        }

        private async UniTaskVoid LoadTutorialPopupAsync()
        {
            var loadingData = _loadingData;
            if (loadingData == null)
                return;

            try
            {
                var tutorialData = CreateTutorialPopupData();
                await TryEnqueuePopupAsync(tutorialData, _cts.Token);
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
                _popupQueue.Dequeue(loadingData);
                if (ReferenceEquals(_loadingData, loadingData))
                    _loadingData = null;
            }
        }

        private TutorialCompletePopupData CreateTutorialPopupData()
        {
            var data = new TutorialCompletePopupData(
                "Tutorial Complete!",
                "Congratulations! You've unlocked new content.");
            data.AddDisposable(data.Activated.Subscribe(_ => _playerState.MarkTutorialComplete()));
            return data;
        }

        private void HandleOfferClicked(string offerId)
        {
            HandleOfferClickedAsync(offerId).Forget();
        }

        private async UniTaskVoid HandleOfferClickedAsync(string offerId)
        {
            if (!_offersModel.TryGetOffer(offerId, out var offer))
                return;

            var data = new OfferPopupData(
                offer.PopupTitle,
                offer.PopupDescription,
                offer.IconPath);

            data.AddDisposable(data.BuyRequested.Subscribe(_ => HandleOfferBought(offerId)));
            await TryEnqueuePopupAsync(data, _cts.Token);
        }

        private void HandleOfferBought(string offerId)
        {
            _playerState.MarkOfferPurchased(offerId);
        }

        private void HandleDebugSystemError()
        {
            TryEnqueuePopupAsync(
                new SystemInterruptPopupData("An unexpected error occurred. Please try again."),
                _cts.Token).Forget();
        }

        private void HandleDebugDailyReward()
        {
            TryEnqueuePopupAsync(
                new DailyRewardPopupData("Bonus coins for you!"),
                _cts.Token).Forget();
        }

        private async UniTask<bool> TryEnqueuePopupAsync(
            BasePopupData data,
            CancellationToken cancellationToken)
        {
            try
            {
                var isEnqueued = await _popupRequestService.EnqueueAsync(data, cancellationToken);
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
            _popupQueue.Enqueue(new SystemInterruptPopupData(RemoteLoadFailureMessage));
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
