using PopupShowcase.MVVM.Common;
using PopupShowcase.MVVM.Popups.Models;
using PopupShowcase.MVVM.Popups.Presenters;
using R3;
using UnityEngine;

namespace PopupShowcase.MVVM.Popups
{
    public abstract class BasePopupView : BaseView<BasePopupModel> { }

    public abstract class BasePopupView<T> : BasePopupView where T : BasePopupModel
    {
        [SerializeField] private PopupTransitionView _transition;

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
