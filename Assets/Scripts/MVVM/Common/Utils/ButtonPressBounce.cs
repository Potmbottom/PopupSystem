using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PopupShowcase.MVVM.Common
{
    [RequireComponent(typeof(Button))]
    public class ButtonPressBounce : BaseTweenAnimation, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private float _pressedScale = 0.92f;
        [SerializeField] private float _pressDuration = 0.08f;
        [SerializeField] private float _releaseDuration = 0.12f;
        [SerializeField] private Ease _pressEase = Ease.OutQuad;
        [SerializeField] private Ease _releaseEase = Ease.OutBack;

        private Vector3 _defaultScale;

        private void Awake()
        {
            _defaultScale = transform.localScale;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            AnimateScale(_defaultScale * _pressedScale, _pressDuration, _pressEase);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            AnimateScale(_defaultScale, _releaseDuration, _releaseEase);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            AnimateScale(_defaultScale, _releaseDuration, _releaseEase);
        }

        private void OnDisable()
        {
            KillTween();
            transform.localScale = _defaultScale;
        }

        private void AnimateScale(Vector3 targetScale, float duration, Ease ease)
        {
            PlayTween(() => transform.DOScale(targetScale, duration).SetEase(ease));
        }
    }
}
