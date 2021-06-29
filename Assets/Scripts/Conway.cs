using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conway
{
	public class Cell
	{
		public int State;
		public Vector2Int Position;
	}

	public class State
	{
		public System.Action<Vector2Int, int> OnCellChanged;

		public Vector2Int Size { get; private set; }
		public int[,] Values { get; private set; }

		public State(Vector2Int size)
		{
			Size = size;
			Values = new int[size.x, size.y];
		}

		public void Set(Vector2Int p, int v)
		{
			Values[p.x, p.y] = v;
			OnCellChanged?.Invoke(p, v);
		}

		public int Get(Vector2Int p)
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

		public System.Action<Vector2Int, int> OnCellChanged;

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
			public int State;
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

		public void SetCellCurrent(Vector2Int p, int v)
		{
			CurrentState.Set(p, v);
			OnCellChanged?.Invoke(p, v);
		}

		public int GetCellCurrent(Vector2Int p)
		{
			return CurrentState.Get(p);
		}

		public void ApplyRule(RuleBase rule)
		{
			ForEachCell(delegate(ForEachCellParams p)
			{
				int x = p.Position.x;
				int y = p.Position.y;

				int v = rule.Apply(this, x, y);
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
