using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using PopupShowcase.Assets;
using PopupShowcase.Meta;
using PopupShowcase.PopupSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace PopupShowcase.Tests.EditMode
{
    public class PopupRequestServiceTests
    {
        [Test]
        public void LocalPopupEntryEnqueuesImmediatelyWithoutRemoteLoad()
        {
            using var provider = new PopupQueueProvider();
            using var remoteContentProvider = new FakeRemoteContentProvider();
            var config = ScriptableObject.CreateInstance<PopupPrefabConfig>();
            var localPrefab = new GameObject("LocalPopupPrefab");
            var popupData = new PopupQueueProviderTests.TestPopupData(PopupType.Login, PopupPriority.Standard);

            config.EntriesByType[PopupType.Login] = new PopupPrefabConfig.Entry
            {
                Type = PopupType.Login,
                Prefab = localPrefab
            };

            try
            {
                var service = new PopupRequestService(provider, config, remoteContentProvider);

                var result = service.EnqueueAsync(popupData).GetAwaiter().GetResult();

                Assert.IsTrue(result);
                Assert.AreEqual(0, remoteContentProvider.LoadCallCount);
                Assert.AreSame(popupData, provider.CurrentItem.CurrentValue.Data);
                Assert.IsNull(provider.CurrentItem.CurrentValue.LoadedPrefab);
            }
            finally
            {
                Object.DestroyImmediate(localPrefab);
                Object.DestroyImmediate(config);
            }
        }

        [Test]
        public void RemotePopupEntryRejectsSecondRequestWhileFirstIsLoading()
        {
            using var provider = new PopupQueueProvider();
            using var remoteContentProvider = new FakeRemoteContentProvider();
            var config = ScriptableObject.CreateInstance<PopupPrefabConfig>();
            var loadedPrefab = new GameObject("RemotePopupPrefab");
            var firstPopup = new PopupQueueProviderTests.TestPopupData(PopupType.TutorialComplete, PopupPriority.Standard);
            var secondPopup = new PopupQueueProviderTests.TestPopupData(PopupType.TutorialComplete, PopupPriority.Standard);

            config.EntriesByType[PopupType.TutorialComplete] = new PopupPrefabConfig.Entry
            {
                Type = PopupType.TutorialComplete,
                RemotePrefab = new RemoteAssetReference("bundles/tutorial", "TutorialCompletePopup")
            };

            try
            {
                var service = new PopupRequestService(provider, config, remoteContentProvider);

                var firstRequest = service.EnqueueAsync(firstPopup);
                var secondResult = service.EnqueueAsync(secondPopup).GetAwaiter().GetResult();

                Assert.AreEqual(1, remoteContentProvider.LoadCallCount);
                Assert.IsFalse(secondResult);
                Assert.AreEqual(1, secondPopup.DisposeCount);
                Assert.IsNull(provider.CurrentItem.CurrentValue);

                remoteContentProvider.CompletePendingLoad(loadedPrefab);
                var firstResult = firstRequest.GetAwaiter().GetResult();

                Assert.IsTrue(firstResult);
                Assert.AreSame(firstPopup, provider.CurrentItem.CurrentValue.Data);
                Assert.AreSame(loadedPrefab, provider.CurrentItem.CurrentValue.LoadedPrefab);

                firstPopup.Close();

                Assert.AreEqual(1, remoteContentProvider.ReleaseCallCount);
                Assert.IsNull(provider.CurrentItem.CurrentValue);
            }
            finally
            {
                Object.DestroyImmediate(loadedPrefab);
                Object.DestroyImmediate(config);
            }
        }

        [Test]
        public void RemotePopupLoadFailureDisposesPopupAndDoesNotEnqueue()
        {
            using var provider = new PopupQueueProvider();
            using var remoteContentProvider = new FakeRemoteContentProvider
            {
                ExceptionToThrow = new InvalidOperationException("load failed")
            };
            var config = ScriptableObject.CreateInstance<PopupPrefabConfig>();
            var popupData = new PopupQueueProviderTests.TestPopupData(PopupType.Offer, PopupPriority.Offer);

            config.EntriesByType[PopupType.Offer] = new PopupPrefabConfig.Entry
            {
                Type = PopupType.Offer,
                RemotePrefab = new RemoteAssetReference("bundles/offers", "OfferPopup")
            };

            try
            {
                var service = new PopupRequestService(provider, config, remoteContentProvider);

                var result = service.EnqueueAsync(popupData).GetAwaiter().GetResult();

                Assert.IsFalse(result);
                Assert.AreEqual(1, popupData.DisposeCount);
                Assert.AreEqual(1, remoteContentProvider.ReleaseCallCount);
                Assert.IsNull(provider.CurrentItem.CurrentValue);
            }
            finally
            {
                Object.DestroyImmediate(config);
            }
        }

        private sealed class FakeRemoteContentProvider : IRemoteContentProvider
        {
            private UniTaskCompletionSource<GameObject> _pendingLoad = new();

            public int LoadCallCount { get; private set; }
            public int ReleaseCallCount { get; private set; }
            public Exception ExceptionToThrow { get; set; }

            public async UniTask<T> LoadAssetAsync<T>(
                RemoteAssetReference reference,
                object handler = null,
                CancellationToken cancellationToken = default)
                where T : Object
            {
                LoadCallCount++;

                if (ExceptionToThrow != null)
                    throw ExceptionToThrow;

                var prefab = await _pendingLoad.Task.AttachExternalCancellation(cancellationToken);
                return prefab as T;
            }

            public void CompletePendingLoad(GameObject prefab)
            {
                _pendingLoad.TrySetResult(prefab);
                _pendingLoad = new UniTaskCompletionSource<GameObject>();
            }

            public void ReleaseHandler(RemoteAssetReference reference, object handler)
            {
                ReleaseCallCount++;
            }

            public void Dispose()
            {
            }
        }
    }
}
