using System;
using System.IO;
using System.Threading;
using Cysharp.Threading.Tasks;
using PopupShowcase.MVVM.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace PopupShowcase.MVVM.ViewModels
{
    public class RemotePlayerStateModel
    {
        public async UniTask<PlayerModel> LoadAsync(string sourcePath, CancellationToken cancellationToken)
        {
            var resolvedUri = ResolveUri(sourcePath);

            using var request = UnityWebRequest.Get(resolvedUri);
            request.SendWebRequest();
            await UniTask.WaitUntil(() => request.isDone, cancellationToken: cancellationToken);

            if (request.result != UnityWebRequest.Result.Success)
                throw new InvalidOperationException(
                    $"Failed to download player state from '{resolvedUri}'. {request.error}");

            var json = request.downloadHandler.text;

            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException($"Player state response from '{resolvedUri}' was empty.");

            var payload = JsonUtility.FromJson<PlayerModel>(json);
            if (payload == null)
                throw new InvalidOperationException($"Player state response from '{resolvedUri}' is invalid.");

            return payload;
        }

        private static string ResolveUri(string sourcePath)
        {
            if (Uri.TryCreate(sourcePath, UriKind.Absolute, out var absoluteUri))
                return absoluteUri.AbsoluteUri;

            var fullPath = Path.Combine(Application.streamingAssetsPath, sourcePath);
            return new Uri(fullPath).AbsoluteUri;
        }
    }
}
