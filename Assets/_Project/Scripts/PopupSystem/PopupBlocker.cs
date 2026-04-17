using PopupShowcase.Core;
using UnityEngine;

namespace PopupShowcase.PopupSystem
{
    public class PopupBlocker : BaseTweenAnimation
    {
        [SerializeField] private CanvasGroup _canvasGroup;

        public void Block()
        {
            gameObject.SetActive(true);
            _canvasGroup.blocksRaycasts = true;
        }

        public void Unblock()
        {
            _canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(false);
        }
    }
}
