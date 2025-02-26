using AYellowpaper.SerializedCollections;
using UnityEngine;

namespace MazeAsset.MazeGenerator
{
    [System.Serializable]
    public class GameObjectManager
    {
        [SerializedDictionary("ID", "GO"), SerializeField]
        internal SerializedDictionary<ID, GameObject> gameObjectManager;

        public GameObjectManager()
        {
            if (gameObjectManager == null)
            {
                gameObjectManager = new SerializedDictionary<ID, GameObject>();
            }
        }

        private ID CreateID(int key1, int key2, int key3)
        {
            return new ID()
            {
                Key1 = key1,
                Key2 = key2,
                Key3 = key3
            };
        }

        public void AddGameObject(GameObject obj, int key1, int key2, int key3)
        {
            var gameObject = CreateID(key1, key2, key3);
            gameObjectManager[gameObject] = obj;
        }

        public GameObject GetGameObject(int key1, int key2, int key3)
        {
            var gameObject = CreateID(key1, key2, key3);
            gameObjectManager.TryGetValue(gameObject, out var obj);
            return obj;
        }

        public bool RemoveGameObject(int key1, int key2, int key3)
        {
            var gameObject = CreateID(key1, key2, key3);
            return gameObjectManager.Remove(gameObject);
        }

        public bool VisibleObject(int key1, int key2, int key3)
        {
            var gameObject = GetGameObject(key1, key2, key3);
            if (gameObject != null)
            {
                gameObject.SetActive(true);
                return true;
            }
            return false;
        }

        public void HideObject(int key1, int key2, int key3)
        {
            var gameObject = GetGameObject(key1, key2, key3);
            gameObject?.SetActive(false);
        }
    }

    [System.Serializable]
    public class ID
    {
        [SerializeField]
        internal int Key1;
        [SerializeField]
        internal int Key2;
        [SerializeField]
        internal int Key3;

        public override bool Equals(object obj)
        {
            if (obj is ID other)
            {
                return Key1 == other.Key1 && Key2 == other.Key2 && Key3 == other.Key3;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Key1.GetHashCode() ^ Key2.GetHashCode() ^ Key3.GetHashCode();
        }
    }
}
