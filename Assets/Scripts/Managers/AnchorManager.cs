
using System.Collections.Generic;
using UnityEngine;
using Core.MiniJSON;

namespace Managers
{
    public class AnchorManager : MonoBehaviour
    {
        public static AnchorManager Instance { get; private set; }

        private HashSet<string> _activeAnchors = new();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                LoadAnchors();
            }
        }

        public void ActivateAnchor(string anchorId)
        {
            if (_activeAnchors.Contains(anchorId)) return;

            _activeAnchors.Add(anchorId);
            SaveAnchors();
        }

        public bool IsAnchorActive(string anchorId)
        {
            return _activeAnchors.Contains(anchorId);
        }

        private void SaveAnchors()
        {
            var anchorList = new List<string>(_activeAnchors);
            var serialized = Json.Serialize(anchorList);
            PlayerPrefs.SetString("Anchors", serialized);
            PlayerPrefs.Save();
        }

        private void LoadAnchors()
        {
            if (PlayerPrefs.HasKey("Anchors"))
            {
                var serialized = PlayerPrefs.GetString("Anchors");
                var deserialized = Json.Deserialize(serialized) as List<object>;
                _activeAnchors = new HashSet<string>();
                foreach (var anchor in deserialized)
                {
                    _activeAnchors.Add(anchor.ToString());
                }
            }
        }
    }
}
