using System;
using System.Collections.Generic;
using System.Linq;
using PopupShowcase.MVVM.Popups.Models;
using UnityEngine;

namespace PopupShowcase.Scriptables
{
    [CreateAssetMenu(menuName = "PopupShowcase/Popup Prefab Config")]
    public class PopupPrefabConfig : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public PopupType Type;
            public GameObject Prefab;
            public string Address;
        }

        [SerializeField] private Entry[] _entries;

        private Dictionary<PopupType, Entry> _cache;

        public Entry Get(PopupType type)
        {
            _cache ??= _entries.ToDictionary(x => x.Type, x => x);
            return _cache.GetValueOrDefault(type);
        }
    }
}
