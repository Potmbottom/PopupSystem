using System.IO;
using UnityEditor;
using UnityEngine;

namespace PopupShowcase.Editor
{
    public static class BuildRemoteBundles
    {
        private const string OutputPath = "Assets/StreamingAssets/AssetBundles";

        [MenuItem("Tools/Build/Remote Bundles")]
        public static void Build()
        {
            Directory.CreateDirectory(OutputPath);

            var manifest = BuildPipeline.BuildAssetBundles(
                OutputPath,
                BuildAssetBundleOptions.None,
                EditorUserBuildSettings.activeBuildTarget);

            if (manifest == null)
            {
                Debug.LogError("Remote bundle build failed.");
                return;
            }

            AssetDatabase.Refresh();
            Debug.Log($"Remote bundles built to: {OutputPath}");
        }
    }
}