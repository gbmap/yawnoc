using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conway
{
	/*******************************
	*       BOARD RULES		
	*/
	public abstract class RuleBase
	{
		public abstract int Apply(Board b, int x, int y);
	}

	public class RuleConway : RuleBase
	{
		public override int Apply(Board b, int cx, int cy)
		{
			Vector2Int cp = new Vector2Int(cx, cy);
			Vector2Int np = new Vector2Int(0, 0);
			int liveNeighbors = 0;
			for (int ny = -1; ny <= 1; ny++)
			{
				for (int nx = -1; nx <= 1; nx++)
				{
					if (nx == 0 && ny == 0) 
						continue;

					np.x = nx;
					np.y = ny;

					Vector2Int pp = cp + np;

					if (pp.x < 0 || pp.x >= b.Size.x ||
						pp.y < 0 || pp.y >= b.Size.y)
						continue;

					liveNeighbors += Convert.ToInt32(b.PreviousState.Get(pp) == 1);
				}
			}

			if (b.PreviousState.Get(cp) == 1) // alive
			{
				return Convert.ToInt32(liveNeighbors >= 2 && liveNeighbors <= 3);
			}
			else
			{
				return Convert.ToInt32(liveNeighbors == 3);
			}
		}
	}

	public class RuleMaintainCell : RuleBase
	{
		public int Value;
		public RuleMaintainCell(int v)
		{
			Value = v;
		}

		public override int Apply(Board b, int cx, int cy)
		{
			Vector2Int p = new Vector2Int(cx, cy);
			if (b.PreviousState.Get(p) == Value)
				return Value;
			return b.CurrentState.Get(p);
		}
	}
}
