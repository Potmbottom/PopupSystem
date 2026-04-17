using System;
using R3;
using UnityEngine;

namespace PopupShowcase.Core
{
    public abstract class BasePresenter : MonoBehaviour, IDisposable
    {
        protected CompositeDisposable Disposables { get; } = new();
        private bool _disposed;

        protected void AddDisposable(IDisposable disposable)
        {
            Disposables.Add(disposable);
        }

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
