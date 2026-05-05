using NUnit.Framework;
using PopupShowcase.MVVM.Popups.Models;
using PopupShowcase.MVVM.Popups.Service;

namespace PopupShowcase.Tests.EditMode
{
    public class PopupQueueProviderTests
    {
        [Test]
        public void ClosingHigherPriorityPopupRestoresPreviousPopup()
        {
            using var provider = new PopupQueueService();
            var standardPopup = new TestPopupModel(PopupType.Login, PopupPriority.Standard);
            var systemPopup = new TestPopupModel(PopupType.SystemInterrupt, PopupPriority.SystemInterrupt);

            provider.Enqueue(standardPopup);
            Assert.AreSame(standardPopup, provider.CurrentItem.CurrentValue.Model);

            provider.Enqueue(systemPopup);
            Assert.AreSame(systemPopup, provider.CurrentItem.CurrentValue.Model);

            systemPopup.Close();

            Assert.AreEqual(1, systemPopup.DisposeCount);
            Assert.AreSame(standardPopup, provider.CurrentItem.CurrentValue.Model);
        }

        [Test]
        public void EnqueueingSamePopupInstanceTwiceThrows()
        {
            using var provider = new PopupQueueService();
            var popup = new TestPopupModel(PopupType.Login, PopupPriority.Standard);

            provider.Enqueue(popup);

            Assert.Throws<System.InvalidOperationException>(() => provider.Enqueue(popup));
            Assert.AreSame(popup, provider.CurrentItem.CurrentValue.Model);
        }

        internal sealed class TestPopupModel : BasePopupModel
        {
            public override PopupType Type { get; }
            public int DisposeCount { get; private set; }

            public TestPopupModel(PopupType type, PopupPriority priority) : base(priority)
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
