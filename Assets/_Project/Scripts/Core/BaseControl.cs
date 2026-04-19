using System;
using System.Collections.Generic;
using R3;
using UnityEngine;

namespace PopupShowcase.Core
{
    public abstract class BaseControl<T> : MonoBehaviour
    {
        protected T Model { get; private set; }

        private readonly CompositeDisposable _modelBindings = new();
        private bool _dirty;
        private bool _bound;

        protected abstract void OnModelUpdate(CompositeDisposable modelBindings);

        protected virtual void OnEnable()
        {
            if (_dirty && _bound)
                RefreshModel();
        }

        protected virtual void OnDisable()
        {
            _modelBindings.Clear();
            if (_bound) _dirty = true;
        }

        protected virtual void OnDestroy()
        {
            _modelBindings.Dispose();
        }

        public void Bind(T model)
        {
            Model = model;
            _bound = true;

            if (isActiveAndEnabled)
                RefreshModel();
            else
                _dirty = true;
        }

        public void Unbind()
        {
            Model = default;
            _bound = false;
            _modelBindings.Clear();
            _dirty = false;
            OnUnbind();
        }

        public void UnbindAndDisableGameObject()
        {
            Unbind();
            if (gameObject.activeSelf)
                gameObject.SetActive(false);
        }

        public void BindAndSetActive(T model)
        {
            if (model != null && !EqualityComparer<T>.Default.Equals(model, default))
            {
                if (!gameObject.activeSelf)
                    gameObject.SetActive(true);
                Bind(model);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        protected virtual void OnUnbind() { }

        private void RefreshModel()
        {
            _modelBindings.Clear();
            if (_bound) OnModelUpdate(_modelBindings);
            _dirty = false;
        }
    }
}
