using UnityEngine;

namespace PopupShowcase.Meta
{
    [CreateAssetMenu(menuName = "PopupShowcase/Game Config")]
    public class GameConfig : ScriptableObject
    {
        [SerializeField] private string _dailyRewardIconPath;
        [SerializeField] private string _defaultSpritePath;
        [SerializeField] private string _playerStateSource = "mock-player-state.json";
        [SerializeField] private int _startupLoadingDelayMs = 1500;
        [SerializeField] private int _mockRemoteDelayMs = 1000;

        public string DailyRewardIconPath => _dailyRewardIconPath;
        public string DefaultSpritePath => _defaultSpritePath;
        public string PlayerStateSource => _playerStateSource;
        public int StartupLoadingDelayMs => Mathf.Max(0, _startupLoadingDelayMs);
        public int MockRemoteDelayMs => Mathf.Max(0, _mockRemoteDelayMs);
    }
}
