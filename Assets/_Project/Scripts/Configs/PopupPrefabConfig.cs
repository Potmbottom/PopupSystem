using System;
using System.Collections.Generic;
using PopupShowcase.Assets;
using PopupShowcase.PopupSystem;
using UnityEngine;

namespace PopupShowcase.Meta
{
    [CreateAssetMenu(menuName = "PopupShowcase/Popup Prefab Config")]
    public class PopupPrefabConfig : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public PopupType Type;
            public GameObject Prefab;
            public RemoteAssetReference RemotePrefab;
        }

        [SerializeField] private Entry[] _entries;
        [NonSerialized] public readonly Dictionary<PopupType, Entry> EntriesByType = new();

        public IReadOnlyList<Entry> Entries => _entries;

        private void OnEnable()
        {
            RebuildEntryCache();
        }

        private void OnValidate()
        {
            RebuildEntryCache();
        }

        private void RebuildEntryCache()
        {
            EntriesByType.Clear();

            if (_entries == null)
                return;

            foreach (var entry in _entries)
                EntriesByType[entry.Type] = entry;
        }
    }
}
