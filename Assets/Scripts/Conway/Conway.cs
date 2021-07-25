using UnityEngine;

using Conway.Rules;

namespace Conway
{
    public enum ECellType
	{
		Obstacle = -1,
		Dead,
		Alive,
		Collectible
	}

	public class State
	{
		public Vector2Int Size { get; private set; }
		public ECellType[,] Values { get; private set; }

		public State(Vector2Int size)
		{
			Size = size;
			Values = new ECellType[size.x, size.y];
		}

		public void Set(Vector2Int p, ECellType v)
		{
			if (!IsPositionValid(p)) 
				return;

			var oldValue = Values[p.x, p.y];
			Values[p.x, p.y] = v;
		}

		public ECellType Get(Vector2Int p)
		{
			if (!IsPositionValid(p)) 
				return ECellType.Dead;
			return Values[p.x, p.y];
		}

		public bool Compare(State other)
		{
			return false;
		}

		public bool IsPositionValid(Vector2Int p)
		{
			return p.x >= 0 && p.x < Size.x && 
				p.y >= 0 && p.y < Size.y;
		}
	}

	public class Board
	{
		public Vector2Int Size = new Vector2Int(16, 16);
		public State CurrentState { get; private set; }
		public State PreviousState { get; private set; }
		public Ruleset Ruleset { get; set; }
		public Config.BoardStyle Style { get; set; }

		public class OnCellChangedParams
		{
			public Vector2Int Position;
			public ECellType  OldType;
			public ECellType  NewType;
		}
		public System.Action<OnCellChangedParams> OnCellChanged;
		public System.Action<Board> OnStep;

		public Board(
			Vector2Int size, 
			Ruleset rules=null,
			Config.BoardStyle style=null
		) {
			Size 		  = size;
			CurrentState  = new State(size);
			PreviousState = new State(size);
			Ruleset 	  = rules;
			Style		  = style;
		}

		public class ForEachCellParams
		{
			public Board Board;
			public Vector2Int Position;
			public ECellType State;
		}

		public void ForEachCell(System.Action<ForEachCellParams> function)
		{
			for (int x = 0; x < Size.x; x++)
			{
				for (int y = 0; y < Size.y; y++)
				{
					Vector2Int p = new Vector2Int(x, y);
					function(new ForEachCellParams
					{
						Board = this,
						Position = p,
						State = CurrentState.Get(p)
					});
				}
			}
		}

		public void SetCellCurrent(Vector2Int p, ECellType v)
		{
			if (!CurrentState.IsPositionValid(p))
				return;

			var oldValue = CurrentState.Get(p);
			CurrentState.Set(p, v);
			OnCellChanged?.Invoke(new OnCellChangedParams {
				Position = p,
				OldType = oldValue,
				NewType = v
			});
		}

		public void SetCellPrevious(Vector2Int p, ECellType v)
		{
			PreviousState.Set(p, v);
		}

		public ECellType GetCellCurrent(Vector2Int p)
		{
			return CurrentState.Get(p);
		}

		public ECellType GetCellPrevious(Vector2Int p)
		{
			return PreviousState.Get(p);
		}

		public bool SetCell(Vector2Int p, ECellType v)
		{
			if (!CurrentState.IsPositionValid(p)) 
				return false;

			SetCellCurrent(p, v);
			SetCellPrevious(p, v);
			return true;
		}

		public void ApplyRule(Rules.RuleBase rule)
		{
			ForEachCell(delegate(ForEachCellParams p)
			{
				int x = p.Position.x;
				int y = p.Position.y;

				ECellType v = rule.Apply(this, x, y);
				if (v == CurrentState.Get(p.Position))
					return; 

				SetCellCurrent(p.Position, v);
			});
		}

		public void StepState()
		{
			ForEachCell(delegate(ForEachCellParams p)
			{
				int x = p.Position.x;
				int y = p.Position.y;

				PreviousState.Set(p.Position, CurrentState.Get(p.Position));
			});

			Ruleset?.Apply(this);
			OnStep?.Invoke(this);
		}
	}

}
