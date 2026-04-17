using System;
using Cysharp.Threading.Tasks;
using PopupShowcase.Assets;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace PopupShowcase.Core
{
    public class ExtendedImage : Image
    {
        private ISpriteLoader _spriteLoader;
        private string _currentSpritePath;
        private int _loadVersion;

        [Inject]
        public void SetDependency(ISpriteLoader spriteLoader)
        {
            _spriteLoader = spriteLoader;
        }

        public async UniTaskVoid LoadSpriteAsync(string spritePath)
        {
            ReplaceSpritePath(spritePath);
            if (string.IsNullOrWhiteSpace(_currentSpritePath))
                return;

            var requestVersion = ++_loadVersion;
            try
            {
                var loadedSprite = await _spriteLoader.LoadAsync(_currentSpritePath, this);
                if (requestVersion != _loadVersion ||
                    !string.Equals(_currentSpritePath, spritePath, StringComparison.Ordinal))
                {
                    _spriteLoader.Release(spritePath, this);
                    return;
                }

                sprite = loadedSprite;
            }
            catch (Exception e)
            {
                Debug.LogError($"[ExtendedImage] Failed to load sprite '{spritePath}': {e.Message}");
            }
        }

        public void Clear()
        {
            ReleaseCurrentSprite();
            sprite = null;
        }

        protected override void OnDestroy()
        {
            ReleaseCurrentSprite();
            base.OnDestroy();
        }

        private void ReplaceSpritePath(string spritePath)
        {
            if (string.Equals(_currentSpritePath, spritePath, StringComparison.Ordinal))
                return;

            ReleaseCurrentSprite();
            _currentSpritePath = spritePath;
            sprite = null;
        }

        private void ReleaseCurrentSprite()
        {
            _loadVersion++;

            if (string.IsNullOrWhiteSpace(_currentSpritePath))
            {
                _currentSpritePath = null;
                return;
            }

            _spriteLoader.Release(_currentSpritePath, this);
            _currentSpritePath = null;
        }
    }
}
