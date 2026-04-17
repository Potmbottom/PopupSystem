using PopupShowcase.Core;
using UnityEngine;
using Zenject;

namespace PopupShowcase.Infrastructure
{
    public class PoolableObject : MonoBehaviour, IPoolable<IMemoryPool>
    {
        private IMemoryPool _pool;
        private Vector2 _initialAnchorMin;
        private Vector2 _initialAnchorMax;
        private Vector2 _initialPivot;

        private void Awake()
        {
            SaveInitialState();
        }

        public void OnSpawned(IMemoryPool pool)
        {
            _pool = pool;
            ResetState();
        }

        public void OnDespawned()
        {
            _pool = null;
        }

        public void Release()
        {
            foreach (var control in GetComponentsInChildren<IUnbindable>(true))
                control.Unbind();

            Despawn();
        }

        public void Despawn()
        {
            _pool?.Despawn(this);
        }

        private void SaveInitialState()
        {
            if (transform is RectTransform rect)
            {
                _initialAnchorMin = rect.anchorMin;
                _initialAnchorMax = rect.anchorMax;
                _initialPivot = rect.pivot;
            }
        }

        private void ResetState()
        {
            if (transform is RectTransform rect)
            {
                rect.anchorMin = _initialAnchorMin;
                rect.anchorMax = _initialAnchorMax;
                rect.pivot = _initialPivot;
            }
        }

        private void OnDestroy()
        {
            OnDespawned();
        }
    }
}
