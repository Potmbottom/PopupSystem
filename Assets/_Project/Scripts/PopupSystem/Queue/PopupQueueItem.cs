using UnityEngine;

namespace PopupShowcase.PopupSystem
{
    public sealed class PopupQueueItem
    {
        public PopupQueueItem(BasePopupData data, GameObject loadedPrefab = null)
        {
            Data = data;
            LoadedPrefab = loadedPrefab;
        }

        public BasePopupData Data { get; }
        public GameObject LoadedPrefab { get; }
    }
}
