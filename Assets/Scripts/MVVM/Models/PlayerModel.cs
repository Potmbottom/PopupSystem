using System;

namespace PopupShowcase.MVVM.Models
{
    [Serializable]
    public class PlayerModel
    {
        public string playerId;
        public bool tutorialCompleted;
        public string[] activeOfferIds;
        public string[] purchasedOfferIds;
    }
}
