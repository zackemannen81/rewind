
using System.Collections.Generic;
using UnityEngine;
using Core.MiniJSON;

namespace Managers
{
    public class KnowledgeManager : MonoBehaviour
    {
        public static KnowledgeManager Instance { get; private set; }

        private Dictionary<string, string> _knowledge = new();

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
                LoadKnowledge();
            }
        }

        public void AddKnowledge(string key, string value)
        {
            _knowledge[key] = value;
            SaveKnowledge();
        }

        public string GetKnowledge(string key)
        {
            _knowledge.TryGetValue(key, out var value);
            return value;
        }

        public bool HasKnowledge(string key)
        {
            return _knowledge.ContainsKey(key);
        }

        private void SaveKnowledge()
        {
            // Simple serialization to PlayerPrefs for now.
            // A more robust solution would use a dedicated save file.
            var serialized = Json.Serialize(_knowledge);
            PlayerPrefs.SetString("Knowledge", serialized);
            PlayerPrefs.Save();
        }

        private void LoadKnowledge()
        {
            if (PlayerPrefs.HasKey("Knowledge"))
            {
                var serialized = PlayerPrefs.GetString("Knowledge");
                var deserialized = Json.Deserialize(serialized) as Dictionary<string, object>;
                _knowledge = new Dictionary<string, string>();
                foreach (var pair in deserialized)
                {
                    _knowledge[pair.Key] = pair.Value.ToString();
                }
            }
        }
    }
}
