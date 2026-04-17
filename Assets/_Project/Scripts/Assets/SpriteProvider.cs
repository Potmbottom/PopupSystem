using Cysharp.Threading.Tasks;
using UnityEngine;

namespace PopupShowcase.Assets
{
    [System.Obsolete("Use ISpriteLoader instead.")]
    public interface ISpriteProvider
    {
        Sprite Load(string address);
        UniTask<Sprite> LoadAsync(string address, object handler = null);
        void Unload(string address, object handler);
    }

    [System.Obsolete("Use ISpriteLoader instead.")]
    public class SpriteProvider : ISpriteProvider
    {
        private readonly ISpriteLoader _spriteLoader;

        public SpriteProvider(ISpriteLoader spriteLoader)
        {
            _spriteLoader = spriteLoader;
        }

        public Sprite Load(string address)
        {
            return _spriteLoader.LoadAsync(address).GetAwaiter().GetResult();
        }

        public UniTask<Sprite> LoadAsync(string address, object handler = null)
        {
            return _spriteLoader.LoadAsync(address, handler);
        }

        public void Unload(string address, object handler)
        {
            _spriteLoader.Release(address, handler);
        }
    }
}
