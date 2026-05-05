using System;

namespace PopupShowcase.MVVM.Models
{
    [Serializable]
    public class PlayerModel
    {
        public bool tutorialCompleted;
        public string[] activeOfferIds;
        public string[] purchasedOfferIds;
    }
}
