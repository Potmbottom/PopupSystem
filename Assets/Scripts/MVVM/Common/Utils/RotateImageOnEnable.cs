using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PopupShowcase.MVVM.Common
{
    [RequireComponent(typeof(Image))]
    public class RotateImageOnEnable : BaseTweenAnimation
    {
        [SerializeField] private float _rotationDuration = 1f;
        [SerializeField] private Ease _rotationEase = Ease.Linear;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = (RectTransform)transform;
        }

        private void OnEnable()
        {
            PlayTween(() => _rectTransform
                .DOLocalRotate(new Vector3(0f, 0f, -360f), _rotationDuration, RotateMode.FastBeyond360)
                .SetEase(_rotationEase)
                .SetLoops(-1, LoopType.Restart));
        }

        private void OnDisable()
        {
            KillTween();
        }
    }
}
