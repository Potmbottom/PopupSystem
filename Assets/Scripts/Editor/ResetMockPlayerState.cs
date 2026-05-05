using System;
using System.IO;
using System.Linq;
using PopupShowcase.Scriptables;
using PopupShowcase.MVVM.Models;
using UnityEditor;
using UnityEngine;

namespace PopupShowcase.Editor
{
    public static class ResetMockPlayerState
    {
        private const string GameConfigPath = "Assets/Resources/Configs/GameConfig.asset";
        private const string OfferCatalogConfigPath = "Assets/Resources/Configs/OfferCatalogConfig.asset";
        private const string DefaultPlayerId = "demo-player";

        [MenuItem("Tools/Debug/Reset Mock Player State")]
        public static void Reset()
        {
            var gameConfig = AssetDatabase.LoadAssetAtPath<GameConfig>(GameConfigPath);
            var offerCatalog = AssetDatabase.LoadAssetAtPath<OfferCatalogConfig>(OfferCatalogConfigPath);

            if (gameConfig == null)
            {
                Debug.LogError($"GameConfig not found at '{GameConfigPath}'.");
                return;
            }

            if (offerCatalog == null)
            {
                Debug.LogError($"OfferCatalogConfig not found at '{OfferCatalogConfigPath}'.");
                return;
            }

            if (Uri.TryCreate(gameConfig.PlayerStateSource, UriKind.Absolute, out _))
            {
                Debug.LogError("Reset Mock Player State only supports local StreamingAssets JSON sources.");
                return;
            }

            var streamingAssetsRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "StreamingAssets"));
            var targetPath = Path.GetFullPath(Path.Combine(streamingAssetsRoot, gameConfig.PlayerStateSource));

            if (!targetPath.StartsWith(streamingAssetsRoot, StringComparison.OrdinalIgnoreCase))
            {
                Debug.LogError($"Resolved player state path '{targetPath}' is outside StreamingAssets.");
                return;
            }

            Directory.CreateDirectory(Path.GetDirectoryName(targetPath) ?? streamingAssetsRoot);

            var payload = new PlayerModel
            {
                playerId = DefaultPlayerId,
                tutorialCompleted = false,
                activeOfferIds = offerCatalog.Offers
                    .Select(offer => offer?.OfferId)
                    .Where(offerId => !string.IsNullOrWhiteSpace(offerId))
                    .ToArray(),
                purchasedOfferIds = Array.Empty<string>()
            };

            File.WriteAllText(targetPath, JsonUtility.ToJson(payload, true));
            AssetDatabase.Refresh();
            Debug.Log($"Reset mock player state: {targetPath}");
        }
    }
}
