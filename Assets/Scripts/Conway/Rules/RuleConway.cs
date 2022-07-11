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

			int liveNeighbors = 0;
			for (int ny = -1; ny <= 1; ny++)
			{
				for (int nx = -1; nx <= 1; nx++)
				{
					// Skip middle cell.
					if (nx == 0 && ny == 0) 
						continue;

					int x = (cx+nx); 
					int y = (cy+ny);

					// Skip cells outside the board.
					if (!b.IsInsideBoard(x,y))
						continue;

					int n = Convert.ToInt32(b.PreviousState.Get(x, y) == Alive);
					liveNeighbors += n;
				}
			}

			if (b.PreviousState.Get(cx, cy) == Alive) 
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
