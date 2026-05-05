using System;
using R3;
using UnityEngine;

namespace PopupShowcase.MVVM.Common
{
    public abstract class BasePresenter : MonoBehaviour, IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new();
        private bool _disposed;

        public virtual void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            Disposables.Dispose();
        }

        protected virtual void OnDestroy()
        {
            Dispose();
        }
    }
}
