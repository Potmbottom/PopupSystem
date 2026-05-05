using System;
using DG.Tweening;
using UnityEngine;

namespace PopupShowcase.MVVM.Common
{
    public abstract class BaseTweenAnimation : MonoBehaviour
    {
        private Tween _activeTween;

        protected void PlayTween(Func<Tween> createTween, Action onComplete = null)
        {
            KillTween();

            var tween = createTween();
            _activeTween = tween;

            if (tween == null)
            {
                onComplete?.Invoke();
                return;
            }

            tween.OnComplete(() =>
            {
                if (_activeTween == tween)
                    _activeTween = null;

                onComplete?.Invoke();
            });

            tween.OnKill(() =>
            {
                if (_activeTween == tween)
                    _activeTween = null;
            });
        }

        protected void KillTween()
        {
            _activeTween?.Kill();
            _activeTween = null;
        }

        protected virtual void OnDestroy()
        {
            KillTween();
        }
    }
}
