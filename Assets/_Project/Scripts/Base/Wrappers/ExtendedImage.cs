using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using PopupShowcase.Assets;
using PopupShowcase.Meta;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PopupShowcase.Core
{
    public class ExtendedImage : Image
    {
        private IAssetProvider _assetProvider;
        private GameConfig _gameConfig;
        private AssetHandle<Sprite> _currentHandle;
        private CancellationTokenSource _cts;

        [Inject]
        public void SetDependency(IAssetProvider assetProvider, GameConfig gameConfig)
        {
            _assetProvider = assetProvider;
            _gameConfig = gameConfig;
        }

        public async UniTaskVoid LoadSpriteAsync(string spritePath)
        {
            CancelCurrent();
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            try
            {
                var handle = await TryLoadAsync(spritePath, token);

                if (handle == null)
                {
                    var fallback = _gameConfig.DefaultSpritePath;
                    if (!string.IsNullOrWhiteSpace(fallback) &&
                        !string.Equals(spritePath, fallback, StringComparison.Ordinal))
                    {
                        handle = await TryLoadAsync(fallback, token);
                    }
                }

                if (token.IsCancellationRequested)
                {
                    handle?.Dispose();
                    return;
                }

                _currentHandle?.Dispose();
                _currentHandle = handle;
                sprite = handle?.Asset;
            }
            catch (OperationCanceledException) { }
        }

        public void Clear()
        {
            CancelCurrent();
            _currentHandle?.Dispose();
            _currentHandle = null;
            sprite = null;
        }

        protected override void OnDestroy()
        {
            Clear();
            base.OnDestroy();
        }

        private async UniTask<AssetHandle<Sprite>> TryLoadAsync(string spritePath, CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(spritePath))
                return null;

            try
            {
                return await _assetProvider.LoadAssetAsync<Sprite>(spritePath, cancellationToken: token);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception exception)
            {
                Debug.LogWarning($"[ExtendedImage] Failed to load sprite '{spritePath}'. {exception.Message}");
                return null;
            }
        }

        private void CancelCurrent()
        {
            if (_cts == null) return;
            _cts.Cancel();
            _cts.Dispose();
            _cts = null;
        }
    }
}
