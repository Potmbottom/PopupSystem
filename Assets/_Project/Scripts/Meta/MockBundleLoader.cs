using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;

namespace PopupShowcase.Meta
{
    public class MockBundleLoader : IDisposable
    {
        private const int ProgressTickMs = 50;
        private const float DelayProgressMax = 0.9f;

        private readonly CancellationTokenSource _cts = new();

        public async UniTask LoadAsync(
            Func<CancellationToken, UniTask> loadOperation,
            int delayMs,
            ReactiveProperty<float> progress = null,
            CancellationToken cancellationToken = default)
        {
            if (loadOperation == null)
                throw new ArgumentNullException(nameof(loadOperation));

            using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(_cts.Token, cancellationToken);
            var ct = linkedCts.Token;
            if (delayMs > 0)
                await RunDelayAsync(delayMs, progress, ct);

            await loadOperation(ct);
            if (progress != null)
                progress.Value = 1f;
        }

        private static async UniTask RunDelayAsync(
            int delayMs,
            ReactiveProperty<float> progress,
            CancellationToken cancellationToken)
        {
            if (progress != null)
                progress.Value = 0f;

            var elapsedMs = 0;
            while (elapsedMs < delayMs)
            {
                var tickMs = Math.Min(ProgressTickMs, delayMs - elapsedMs);
                await UniTask.Delay(tickMs, cancellationToken: cancellationToken);
                elapsedMs += tickMs;

                if (progress != null)
                    progress.Value = (elapsedMs / (float)delayMs) * DelayProgressMax;
            }
        }

        public void Dispose()
        {
            _cts.Cancel();
            _cts.Dispose();
        }
    }
}
