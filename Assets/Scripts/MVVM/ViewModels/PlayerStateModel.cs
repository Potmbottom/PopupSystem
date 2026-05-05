using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using PopupShowcase.MVVM.Models;
using PopupShowcase.Scriptables;
using R3;
using UnityEngine;

namespace PopupShowcase.MVVM.ViewModels
{
    public class PlayerStateModel : IDisposable
    {
        private readonly GameConfig _gameConfig;
        private readonly OfferCatalogConfig _offerCatalogConfig;
        private readonly RemotePlayerStateModel _remoteLoader;
        private readonly HashSet<string> _activeOfferIds = new();
        private readonly HashSet<string> _purchasedOfferIds = new();
        private readonly ReactiveProperty<bool> _tutorialCompleted = new(false);
        private readonly Subject<Unit> _offersChanged = new();

        public string PlayerId { get; private set; } = "local-player";
        public ReadOnlyReactiveProperty<bool> TutorialCompleted => _tutorialCompleted;
        public Observable<Unit> OffersChanged => _offersChanged;

        public PlayerStateModel(
            GameConfig gameConfig,
            OfferCatalogConfig offerCatalogConfig,
            RemotePlayerStateModel remoteLoader)
        {
            _gameConfig = gameConfig;
            _offerCatalogConfig = offerCatalogConfig;
            _remoteLoader = remoteLoader;
        }

        public async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            PlayerModel payload;

            try
            {
                payload = await _remoteLoader.LoadAsync(_gameConfig.PlayerStateSource, cancellationToken);
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    $"[PlayerStateModel] Falling back to local defaults because player state could not be loaded. {exception.Message}");
                payload = CreateFallbackPayload();
            }

            ApplyPayload(payload);
        }

        public bool IsOfferActive(string offerId)
        {
            return _activeOfferIds.Contains(offerId);
        }

        public bool IsOfferPurchased(string offerId)
        {
            return _purchasedOfferIds.Contains(offerId);
        }

        public void MarkTutorialComplete()
        {
            if (_tutorialCompleted.Value)
                return;

            _tutorialCompleted.Value = true;
            NotifyOffersChanged();
        }

        public void MarkOfferPurchased(string offerId)
        {
            if (string.IsNullOrWhiteSpace(offerId))
                return;

            if (_purchasedOfferIds.Add(offerId))
                NotifyOffersChanged();
        }

        public void Dispose()
        {
            _tutorialCompleted.Dispose();
            _offersChanged.Dispose();
        }

        private void ApplyPayload(PlayerModel payload)
        {
            _activeOfferIds.Clear();
            _purchasedOfferIds.Clear();

            PlayerId = string.IsNullOrWhiteSpace(payload.playerId) ? "local-player" : payload.playerId;
            _tutorialCompleted.Value = payload.tutorialCompleted;

            AddRange(_activeOfferIds, payload.activeOfferIds);
            AddRange(_purchasedOfferIds, payload.purchasedOfferIds);

            NotifyOffersChanged();
        }

        private PlayerModel CreateFallbackPayload()
        {
            var offers = _offerCatalogConfig.Offers;
            var activeOffers = new string[offers.Count];

            for (int i = 0; i < offers.Count; i++)
                activeOffers[i] = offers[i].OfferId;

            return new PlayerModel
            {
                playerId = "local-player",
                tutorialCompleted = false,
                activeOfferIds = activeOffers,
                purchasedOfferIds = Array.Empty<string>()
            };
        }

        private void NotifyOffersChanged()
        {
            _offersChanged.OnNext(Unit.Default);
        }

        private static void AddRange(HashSet<string> target, string[] values)
        {
            if (values == null)
                return;

            foreach (var value in values)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    target.Add(value);
            }
        }
    }
}
