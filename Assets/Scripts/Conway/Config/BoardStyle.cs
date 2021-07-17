using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Conway.Config
{
    [CreateAssetMenu(menuName="Conway/Config/Board Style", fileName ="BoardStyle")]
    public class BoardStyle : ScriptableObject
    {
		private static bool CompareColor(Color a, Color b)
		{
			return Mathf.Approximately(Vector4.Distance(a, b), 0.0f);
		}

        public static BoardStyle Default
        {
            get
            {
				var str = "Data/BoardStyles/BoardStyleDefault";
				return Resources.Load<Config.BoardStyle>(str);
            }
        }

        [System.Serializable]
        public class CellColorPair
        {
            public Conway.ECellType Type;
            public Color Color;
        }

        public List<CellColorPair> Config;

        public Color GetColor(Conway.ECellType Type)
        {
            var ccp = Config.FirstOrDefault(x=>x.Type == Type);
            if (ccp == null)
            {
                string err = $"Couldn't find {Type} config.";
                throw new System.Exception(err);
            }

            return ccp.Color;
        }

        public ECellType GetType(Color color)
        {
            var ccp = Config.FirstOrDefault(x=>CompareColor(x.Color, color));
            if (ccp == null)
            {
                string err = $"Couldn't find {color} config.";
                throw new System.Exception(err);
            }

            return ccp.Type;
        }
    }
}