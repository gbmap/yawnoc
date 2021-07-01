using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conway
{
	public enum ECellType
	{
		Obstacle = -1,
		Dead,
		Alive,
		Collectible
	}

	public class Cell
	{
		public int State;
		public Vector2Int Position;
	}

	public class State
	{
		public System.Action<Vector2Int, ECellType> OnCellChanged;

		public Vector2Int Size { get; private set; }
		public ECellType[,] Values { get; private set; }

		public State(Vector2Int size)
		{
			Size = size;
			Values = new ECellType[size.x, size.y];
		}

		public void Set(Vector2Int p, ECellType v)
		{
			Values[p.x, p.y] = v;
			OnCellChanged?.Invoke(p, v);
		}

		public ECellType Get(Vector2Int p)
		{
			return Values[p.x, p.y];
		}

		public bool Compare(State other)
		{
			return false;
		}
	}

	public class Board
	{
		public Vector2Int Size = new Vector2Int(16, 16);
		public State CurrentState { get; private set; }
		public State PreviousState { get; private set; }

		public System.Action<Vector2Int, ECellType> OnCellChanged;

		public Board(Vector2Int size)
		{
			Size 		  = size;
			CurrentState  = new State(size);
			PreviousState = new State(size);
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
			CurrentState.Set(p, v);
			OnCellChanged?.Invoke(p, v);
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

		public void SetCell(Vector2Int p, ECellType v)
		{
			SetCellCurrent(p, v);
			SetCellPrevious(p, v);
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

				CurrentState.Set(p.Position, v);
				OnCellChanged?.Invoke(p.Position, v);
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
		}
	}

}
