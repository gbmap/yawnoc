using System;
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

	public class BoardLoader 
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

	public class LevelLoader : MonoBehaviour
	{
		[Header("Manual Creation")]
		public Vector2Int Size = new Vector2Int(16, 16);

		[Header("Loading")]
		public Data.Level Level;
		public Conway.Rules.Ruleset Ruleset;

		private BoardComponent   _boardComponent;
		private BuilderComponent _builderComponent;
		
		void Start()
		{
			if (Level == null) return;
			LoadBoard(Level, Ruleset);
			LoadBuilder(Level);
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.L) && Level != null)
			{
				Load(Level, Ruleset);
			}
				
			if (Input.GetKeyDown(KeyCode.C))
				_boardComponent.GenerateBoard(new Conway.Board(Size, Ruleset));
		}

		void Load(Data.Level level, Conway.Rules.Ruleset ruleset)
		{
			LoadBoard(level, ruleset);
			LoadBuilder(level);
		}

		void LoadBoard(Data.Level level, Conway.Rules.Ruleset ruleset)
		{
			if (_boardComponent == null)
			{
				BoardComponent boardComponent = FindObjectOfType<BoardComponent>();
				if (boardComponent == null)
					throw new Exception("Scene must have a Board object with BoardComponent attached to it.");

				_boardComponent = boardComponent;
			}

			Conway.Board board = BoardLoader.Load(level);
			board.Ruleset = ruleset;

			_boardComponent.GenerateBoard(board);
		}

		void LoadBuilder(Data.Level level)
		{
			if (_builderComponent == null)
			{
				var builderComponent = FindObjectOfType<BuilderComponent>();
				if (builderComponent == null)
				{
					GameObject builder = new GameObject("Builder");
					builderComponent = builder.AddComponent<BuilderComponent>();
				}
				_builderComponent = builderComponent;
			}

			_builderComponent.SetResources(new Level.BuildResources(level.Resources));
		}
	}
}
