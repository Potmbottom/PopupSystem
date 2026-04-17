using PopupShowcase.Core;
using R3;
using UnityEngine;

namespace PopupShowcase.PopupSystem
{
    public abstract class BasePopupControl : BaseControl<BasePopupData> { }

    public abstract class BasePopupControl<T> : BasePopupControl where T : BasePopupData
    {
        [SerializeField] private PopupTransition _transition;

        protected T PopupModel { get; private set; }

        protected sealed override void OnModelUpdate(CompositeDisposable modelBindings)
        {
            PopupModel = Model as T;
            Debug.Assert(PopupModel != null, $"Expected model of type {typeof(T).Name}");

            if (_transition != null)
                _transition.AnimateOpen(() => Model.TransitionToActive());
            else
                Model.TransitionToActive();

            OnPopupModelUpdate(modelBindings);
        }

        protected virtual void OnPopupModelUpdate(CompositeDisposable modelBindings) { }

        protected void ClosePopup()
        {
            if (_transition != null)
            {
                _transition.AnimateClose(() => Model.Close());
                return;
            }

            Model.Close();
        }
    }
}
