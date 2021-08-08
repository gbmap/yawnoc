using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conway.Data
{
    [CreateAssetMenu(menuName ="Conway/Level/Collection", fileName="LevelCollection")]
    public class LevelCollection : ScriptableObject
    {
        public Level[] Levels;

        public bool NextLevel(Level l, out Level next)
        {
            next = null;

            int index = System.Array.IndexOf(Levels, l);
            if (index == Levels.Length - 1 || index == -1)
                return false;

            next = Levels[index+1];
            return true;
        }
    }
}