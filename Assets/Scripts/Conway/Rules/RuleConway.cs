using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conway.Rules
{
	[CreateAssetMenu(menuName="Conway/Rules/Conway", fileName="Conway")]
	public class RuleConway : RuleBase
	{
		public ECellType Alive;
		public ECellType Dead;

		public override ECellType Apply(Board b, int cx, int cy)
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

					liveNeighbors += Convert.ToInt32(b.PreviousState.Get(pp) == Alive);
				}
			}

			if (b.PreviousState.Get(cp) == Alive) // alive
			{
				bool alive = (liveNeighbors >= 2 && liveNeighbors <= 3);
				return  alive ? Alive : Dead;
			} 
			else
			{
				bool alive = liveNeighbors == 3;
				return alive ? Alive : b.GetCellPrevious(cp);
			}
		}
	}
}
