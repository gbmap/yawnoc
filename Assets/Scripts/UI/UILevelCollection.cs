using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UILevelCollection : MonoBehaviour
    {
        public GameObject    LevelInfo;
        public RectTransform Container;

        public Conway.LevelLoader LevelLoader;

        private List<GameObject> Levels;

        void Awake()
        {
            Levels = new List<GameObject>();
        }

        void Start()
        {
            SetLevelCollection(LevelLoader.LevelCollection);
        }

        public void SetLevelCollection(Conway.Data.LevelCollection collection)
        {
            Levels.ForEach(x=> Destroy(x));
            Levels.Clear();

            int nLevels = collection.Levels.Length;
            for (int i = 0; i < nLevels; i++)
            {
                var instance = Instantiate(LevelInfo, Container);

                UILevelInfo info = instance.GetComponent<UILevelInfo>();
                info.Level       = collection.Levels[i];

                Levels.Add(instance);
            }
        }
    }
}