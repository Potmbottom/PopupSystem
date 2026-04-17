using System;

namespace PopupShowcase.Meta.Models
{
    [Serializable]
    public class PlayerStatePayload
    {
        public string playerId;
        public bool tutorialCompleted;
        public string[] activeOfferIds;
        public string[] purchasedOfferIds;
    }
}
