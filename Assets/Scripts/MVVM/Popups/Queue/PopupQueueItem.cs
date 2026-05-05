using PopupShowcase.MVVM.Popups.Models;
using UnityEngine;

namespace PopupShowcase.MVVM.Popups
{
    public sealed class PopupQueueItem
    {
        public PopupQueueItem(BasePopupModel model, GameObject loadedPrefab = null)
        {
            Model = model;
            LoadedPrefab = loadedPrefab;
        }

        public BasePopupModel Model { get; }
        public GameObject LoadedPrefab { get; }
    }
}
