using UnityEngine;

namespace PopupShowcase.MVVM.Popups.Models
{
    public sealed class QueueModel
    {
        public QueueModel(BasePopupModel model, GameObject loadedPrefab = null)
        {
            Model = model;
            LoadedPrefab = loadedPrefab;
        }

        public BasePopupModel Model { get; }
        public GameObject LoadedPrefab { get; }
    }
}