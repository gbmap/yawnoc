using System;
using Frictionless;
using Messages.Gameplay;
using UnityEngine;

namespace Conway
{
    public enum ELevelMode
	{
		Collect,
		None
	}

	public class BoardLoader 
	{
		private static bool CompareColor(Color a, Color b)
		{
			return Mathf.Approximately(Vector4.Distance(a, b), 0.0f);
		}
	}

	public class LevelLoader : MonoBehaviour
	{
		private static LevelLoader _instance;
		public static LevelLoader Instance
		{
			get { return (_instance ?? (_instance = FindObjectOfType<LevelLoader>()));  }
		}

		[Header("Manual Creation")]
		public Vector2Int Size = new Vector2Int(16, 16);

		[Header("Loading")]
		public Data.Level 		 	Level;
		public Data.LevelCollection LevelCollection;
		public Rules.Ruleset     	Ruleset;
		public Config.BoardStyle 	Style;

		private BoardComponent   _boardComponent;
		private BuilderComponent _builderComponent;
		
		void Start()
		{
			if (Level == null) return;
			if (Style == null)
				Style = Config.BoardStyle.Default;

			Load(Level, Ruleset, Style, false);
		}

		void Update()
		{
			if (Input.GetKeyDown(KeyCode.L) && Level != null)
				MessageRouter.RaiseMessage(new Messages.Gameplay.ResetLevel { UpdateUIState=true });
				
			if (Input.GetKeyDown(KeyCode.C))
				_boardComponent.GenerateBoard(new Conway.Board(Size, Ruleset));
		}

		void Load(
			Data.Level level, 
			Rules.Ruleset ruleset,
			Config.BoardStyle style,
			bool updateUIState
		) {
			LoadBoard(level, ruleset, style);
			LoadGamemode(_boardComponent, level);
			LoadBuilder(level);

			if (updateUIState)
			{
				MessageRouter.RaiseMessage(new Messages.UI.OnChangeState {
					State = UI.EUIState.Gameplay
				});
			}
		}

		void LoadBoard(
			Data.Level level, 
			Rules.Ruleset ruleset,
			Config.BoardStyle style
		) {
			if (_boardComponent == null)
			{
				BoardComponent boardComponent = FindObjectOfType<BoardComponent>();
				if (boardComponent == null)
					throw new Exception("Scene must have a Board object with BoardComponent attached to it.");

				_boardComponent = boardComponent;
			}

			Conway.Board board = level.Load(style);
			board.Ruleset	   = ruleset;
			board.Style  	   = style;

			_boardComponent.GenerateBoard(board);

			Level = level;
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

			_builderComponent.SetResources(new Conway.Builder.BuildResources(level.Resources));
		}

		void LoadGamemode(BoardComponent component, Data.Level level)
		{
			if (level.Gamemode == null)
				return;

			var gamemode = gameObject.GetComponent<GamemodeComponent>();

			if (!gamemode)
				gamemode = gameObject.AddComponent<GamemodeComponent>();

			var rsrcGamemode = Resources.Load<Gamemode>("Data/Gamemodes/GamemodeEliminateCell");
			gamemode.Begin(rsrcGamemode, component);
		}

		void OnEnable()
		{
			MessageRouter.AddHandler<Messages.Gameplay.LoadLevel>(Cb_LoadLevel);
			MessageRouter.AddHandler<Messages.Gameplay.ResetLevel>(Cb_ResetLevel);
		}

		void OnDisable()
		{
			MessageRouter.RemoveHandler<Messages.Gameplay.LoadLevel>(Cb_LoadLevel);
			MessageRouter.RemoveHandler<Messages.Gameplay.ResetLevel>(Cb_ResetLevel);
		}

        private void Cb_LoadLevel(LoadLevel msg)
        {
			Load(msg.Level, Ruleset, Style, msg.UpdateUIState);
        }

        private void Cb_ResetLevel(ResetLevel msg)
        {
			Load(this.Level, Ruleset, Style, msg.UpdateUIState);
        }

    }
}
