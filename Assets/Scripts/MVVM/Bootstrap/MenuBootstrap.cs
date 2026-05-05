using UnityEngine;
using Zenject;

namespace PopupShowcase.MVVM.Bootstrap
{
    public class MenuBootstrap : MonoBehaviour
    {
        [SerializeField] private GameObject _menuCanvasPrefab;

        private DiContainer _container;

        [Inject]
        public void SetDependency(DiContainer container)
        {
            _container = container;
        }

        private void Awake()
        {
            _container.InstantiatePrefab(_menuCanvasPrefab);
        }
    }
}
