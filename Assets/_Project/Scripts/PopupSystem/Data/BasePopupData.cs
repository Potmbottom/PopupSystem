using System;
using R3;

namespace PopupShowcase.PopupSystem
{
    public abstract class BasePopupData : IDisposable
    {
        public abstract PopupType Type { get; }
        public PopupPriority Priority { get; }
        public Observable<Unit> Activated => _activatedSubject;
        public Observable<Unit> Closed => _closedSubject;

        private readonly Subject<Unit> _activatedSubject = new();
        private readonly Subject<Unit> _closedSubject = new();
        private readonly CompositeDisposable _disposables = new();
        private bool _isClosed;
        private bool _isDisposed;

        protected BasePopupData(PopupPriority priority)
        {
            Priority = priority;
        }

        public void TransitionToActive()
        {
            if (_isClosed || _isDisposed) return;
            _activatedSubject.OnNext(Unit.Default);
        }

        public virtual void Close()
        {
            if (_isClosed || _isDisposed) return;
            _isClosed = true;
            _closedSubject.OnNext(Unit.Default);
        }

        public void AddDisposable(IDisposable disposable)
        {
            _disposables.Add(disposable);
        }

        public virtual void Dispose()
        {
            if (_isDisposed) return;
            _isDisposed = true;
            _isClosed = true;
            _disposables.Dispose();
        }
    }
}
