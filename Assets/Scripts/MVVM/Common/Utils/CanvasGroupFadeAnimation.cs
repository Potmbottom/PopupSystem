using System;
using DG.Tweening;
using UnityEngine;

namespace PopupShowcase.MVVM.Common
{
    public class CanvasGroupFadeAnimation : BaseTweenAnimation
    {
        [SerializeField] private CanvasGroup _canvasGroup;
        [SerializeField] private float _fadeDuration = 0.25f;
        [SerializeField] private Ease _showEase = Ease.OutQuad;
        [SerializeField] private Ease _hideEase = Ease.InQuad;

        public void ShowInstant()
        {
            KillTween();
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;
        }

        public void HideInstant()
        {
            KillTween();
            _canvasGroup.alpha = 0f;
            _canvasGroup.gameObject.SetActive(false);
        }

        public void FadeIn(Action onComplete = null)
        {
            _canvasGroup.gameObject.SetActive(true);
            _canvasGroup.alpha = 0f;

            PlayTween(
                () => DOTween.To(
                        () => _canvasGroup.alpha,
                        value => _canvasGroup.alpha = value,
                        1f,
                        _fadeDuration)
                    .SetEase(_showEase),
                onComplete);
        }

        public void FadeOutAndDisable(Action onComplete = null)
        {
            PlayTween(
                () => DOTween.To(
                        () => _canvasGroup.alpha,
                        value => _canvasGroup.alpha = value,
                        0f,
                        _fadeDuration)
                    .SetEase(_hideEase),
                () =>
                {
                    _canvasGroup.gameObject.SetActive(false);
                    onComplete?.Invoke();
                });
        }

    }
}
