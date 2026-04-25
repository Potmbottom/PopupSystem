using UnityEngine;

namespace PopupShowcase.Meta
{
    [CreateAssetMenu(menuName = "PopupShowcase/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private string _defaultSpritePath;
        [SerializeField] private string _playerStateSource = "mock-player-state.json";
        [SerializeField] private int _startupLoadingDelayMs = 1500;

        public string DefaultSpritePath => _defaultSpritePath;
        public string PlayerStateSource => _playerStateSource;
        public int StartupLoadingDelayMs => Mathf.Max(0, _startupLoadingDelayMs);
    }
}
