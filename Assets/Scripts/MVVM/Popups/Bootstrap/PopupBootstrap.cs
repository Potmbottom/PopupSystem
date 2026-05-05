using UnityEngine;
using Zenject;

namespace PopupShowcase.MVVM.Popups.Bootstrap
{
    public class PopupBootstrap : MonoBehaviour
    {
        [SerializeField] private GameObject _popupCanvasPrefab;

        private DiContainer _container;

        [Inject]
        public void SetDependency(DiContainer container)
        {
            _container = container;
        }

        private void Awake()
        {
            var canvas = _container.InstantiatePrefab(_popupCanvasPrefab);
            DontDestroyOnLoad(canvas);
        }
    }
}
