using System;
using DG.Tweening;
using PopupShowcase.Core;
using UnityEngine;

namespace PopupShowcase.PopupSystem
{
    public class PopupTransition : BaseTweenAnimation
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

        public void SetVisible(bool visible)
        {
            KillTween();
            _canvasGroup.alpha = visible ? 1f : 0f;
        }
    }
}
