using System;
using DG.Tweening;
using PopupShowcase.MVVM.Common;
using UnityEngine;

namespace PopupShowcase.MVVM.Popups.Presenters
{
    public class PopupTransitionView : BaseTweenAnimation
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration = 0.25f;
        [SerializeField] private Ease _openEase = Ease.OutQuad;
        [SerializeField] private Ease _closeEase = Ease.InQuad;

        public void AnimateOpen(Action onComplete = null)
        {
            _canvasGroup.alpha = 0f;
            PlayTween(
                () => DOTween.To(
                        () => _canvasGroup.alpha,
                        value => _canvasGroup.alpha = value,
                        1f,
                        _fadeDuration)
                    .SetEase(_openEase),
                onComplete);
        }

        public void AnimateClose(Action onComplete = null)
        {
            PlayTween(
                () => DOTween.To(
                        () => _canvasGroup.alpha,
                        value => _canvasGroup.alpha = value,
                        0f,
                        _fadeDuration)
                    .SetEase(_closeEase),
                onComplete);
        }
    }
}
