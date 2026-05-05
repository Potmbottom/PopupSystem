using System;
using System.Collections.Generic;
using UnityEngine;

namespace PopupShowcase.Scriptables
{
    [CreateAssetMenu(menuName = "PopupShowcase/Offer Catalog Config")]
    public class OfferCatalogConfig : ScriptableObject
    {
        [SerializeField] private OfferConfig[] _offers;

        public IReadOnlyList<OfferConfig> Offers => _offers ?? Array.Empty<OfferConfig>();
    }
}
