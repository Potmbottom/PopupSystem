using System;
using UnityEngine;

namespace PopupShowcase.Scriptables
{
    
    [Serializable]
    public class OfferConfig
    {
        [SerializeField] private string _offerId;
        [SerializeField] private string _cardText;
        [SerializeField] private string _popupTitle;
        [SerializeField] private string _popupDescription;
        [SerializeField] private bool _requiresTutorialComplete;
        [SerializeField] private string _iconPath;

        public string OfferId => _offerId;
        public string CardText => _cardText;
        public string PopupTitle => _popupTitle;
        public string PopupDescription => _popupDescription;
        public bool RequiresTutorialComplete => _requiresTutorialComplete;
        public string IconPath => _iconPath;
    }
}
