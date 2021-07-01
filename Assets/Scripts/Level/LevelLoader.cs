using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellType = Conway.ECellType;

namespace Gameplay 
{
	public enum EMode
	{
		Collect
	}

	public class LevelLoader 
	{
		public static string GetLevelPath(EMode gameMode, uint index)
		{
			return $"{gameMode.ToString()}/{index}";
			
		}
		
		public static Conway.Board Load(Data.Level level)
		{
			return LoadBoardFromTexture(level.Texture);
		}

		public static ECellType ColorToCell(Color color)
		{
			if (CompareColor(color, Color.white)) return ECellType.Dead;
			if (CompareColor(color, Color.black)) return ECellType.Alive;
			if (CompareColor(color, Color.green)) return ECellType.Collectible;
			if (CompareColor(color, Color.red)) return ECellType.Obstacle;
			Debug.Log("TEST");
			return ECellType.Dead;
		}

		public static Color CellToColor(ECellType cell)
		{
			switch (cell)
			{
				case ECellType.Alive: return Color.black;
				case ECellType.Dead: return Color.white;
				case ECellType.Obstacle: return Color.red;
				case ECellType.Collectible: return Color.green;
			}
			return Color.white;
		}

		private static bool CompareColor(Color a, Color b)
		{
			return Mathf.Approximately(Vector4.Distance(a, b), 0.0f);
		}

		public static Conway.Board LoadBoardFromTexture(Texture2D texture)
		{
			Conway.Board b = new Conway.Board(new Vector2Int(texture.width, texture.height));
			for (int x = 0; x < texture.width; x++)
			{
				for (int y = 0; y < texture.height; y++)
				{
					Color clr = texture.GetPixel(x, y);
					b.SetCell(new Vector2Int(x, y), ColorToCell(clr));
				}
			}
			
			return b;
		}
	}
}
