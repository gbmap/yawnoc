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
			// Vector2Int cp = new Vector2Int(cx, cy);
			// Vector2Int np = new Vector2Int(0, 0);
			int liveNeighbors = 0;
			for (int ny = -1; ny <= 1; ny++)
			{
				for (int nx = -1; nx <= 1; nx++)
				{
					if (nx == 0 && ny == 0) 
						continue;

					// np.x = nx;
					// np.y = ny;
					int x = (cx + nx); // % b.Size.x;
					int y = (cy +ny); // % b.Size.y;

					// Vector2Int pp = cp + np;

					if (x < 0 || x >= b.Size.x ||
						y < 0 || y >= b.Size.y)
						continue;

					int n = Convert.ToInt32(b.PreviousState.Get(x, y) == Alive);
					liveNeighbors += n;
				}
			}

			if (b.PreviousState.Get(cx, cy) == Alive) // alive
			{
				bool alive = (liveNeighbors >= 2 && liveNeighbors <= 3);
				return  alive ? Alive : Dead;
			} 
			else
			{
				bool alive = liveNeighbors == 3;
				return alive ? Alive : b.PreviousState.Get(cx, cy);
			}
		}
	}
}
