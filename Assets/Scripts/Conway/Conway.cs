using UnityEngine;
using System.Linq;

using Conway.Rules;
using UnityEngine.Profiling;
using Frictionless;

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
			Set(p.x, p.y, v);
		}

		public void Set(int x, int y, ECellType v)
		{
			if (!IsPositionValid(x, y)) 
				return;

			var oldValue = Values[x, y];
			Values[x, y] = v;
		}

		public ECellType Get(Vector2Int p)
		{
			return Get(p.x, p.y);
		}

		public ECellType Get(int x, int y)
		{
			if (!IsPositionValid(x, y)) 
				return ECellType.Dead;
			return Values[x, y];
		}

		public bool Compare(State other)
		{
			return false;
		}

		public bool IsPositionValid(Vector2Int p)
		{
			return IsPositionValid(p.x, p.y);
		}

		public bool IsPositionValid(int x, int y)
		{
			return x >= 0 && x < Size.x && 
				y >= 0 && y < Size.y;
		}

		public void Copy(State other)
		{
			Values = (ECellType[,])other.Values.Clone();
		}
	}

	/*
		Where the simulation should run based 
		on live cells.
	*/
	public class SimulationBounds
	{
		public RectInt Bounds { get; set; }

		public static SimulationBounds GenerateBounds(Board b)
		{
			//b.CurrentState
			return new SimulationBounds{};
		}

		public static SimulationBounds GenerateFullBounds(Board b)
		{
			return new SimulationBounds {};

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

		public void SetCellCurrent(int x, int y, ECellType v)
		{
			if (!CurrentState.IsPositionValid(x, y))
				return;

			var oldValue = CurrentState.Get(x, y);
			if (oldValue == v)
				return; 

			CurrentState.Set(x, y, v);

			MessageRouter.RaiseMessage(new Messages.Board.OnCellChanged {
				Position = new Vector2Int(x, y),
				OldType = oldValue,
				NewType = v
			});
		}

		public void SetCellPrevious(int x, int y, ECellType v)
		{
			PreviousState.Set(x, y, v);
		}

		public ECellType GetCellCurrent(int x, int y)
		{
			return CurrentState.Get(x, y);
		}

		public ECellType GetCellPrevious(Vector2Int p)
		{
			return PreviousState.Get(p);
		}

		public bool SetCell(Vector2Int p, ECellType v) 
		{
			if (!CurrentState.IsPositionValid(p)) 
				return false;

			SetCellCurrent(p.x, p.y, v);
			SetCellPrevious(p.x, p.y, v);
			return true;
		}

		public void ApplyRule(Rules.RuleBase rule, int x, int y)
		{
			ECellType v = rule.Apply(this, x, y);
			if (v == CurrentState.Get(x, y))
				return; 

			SetCellCurrent(x, y, v);
		}

		public void StepState()
		{
			PreviousState.Copy(CurrentState);

			Profiler.BeginSample("Ruleset");
			{
				Ruleset?.Apply(this);
			}
			Profiler.EndSample();
		}

		public bool IsInsideBoard(int x, int y)
		{
			return x >= 0 && x < Size.x && y >= 0 && y < Size.y;
		}
	}

}
