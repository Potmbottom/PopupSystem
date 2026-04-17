using NUnit.Framework;
using PopupShowcase.PopupSystem;

namespace PopupShowcase.Tests.EditMode
{
    public class PopupQueueProviderTests
    {
        [Test]
        public void ClosingHigherPriorityPopupRestoresPreviousPopup()
        {
            using var provider = new PopupQueueProvider();
            var standardPopup = new TestPopupData(PopupType.Login, PopupPriority.Standard);
            var systemPopup = new TestPopupData(PopupType.SystemInterrupt, PopupPriority.SystemInterrupt);

            provider.Enqueue(standardPopup);
            Assert.AreSame(standardPopup, provider.CurrentItem.CurrentValue.Data);

            provider.Enqueue(systemPopup);
            Assert.AreSame(systemPopup, provider.CurrentItem.CurrentValue.Data);

            systemPopup.Close();

            Assert.AreEqual(1, systemPopup.DisposeCount);
            Assert.AreSame(standardPopup, provider.CurrentItem.CurrentValue.Data);
        }

        [Test]
        public void EnqueueingSamePopupInstanceTwiceThrows()
        {
            using var provider = new PopupQueueProvider();
            var popup = new TestPopupData(PopupType.Login, PopupPriority.Standard);

            provider.Enqueue(popup);

            Assert.Throws<System.InvalidOperationException>(() => provider.Enqueue(popup));
            Assert.AreSame(popup, provider.CurrentItem.CurrentValue.Data);
        }

        internal sealed class TestPopupData : BasePopupData
        {
            public override PopupType Type { get; }
            public int DisposeCount { get; private set; }

            public TestPopupData(PopupType type, PopupPriority priority) : base(priority)
            {
                Type = type;
            }

            public override void Dispose()
            {
                DisposeCount++;
                base.Dispose();
            }
        }
    }
}
