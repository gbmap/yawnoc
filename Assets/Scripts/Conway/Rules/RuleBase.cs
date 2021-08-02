using UnityEngine;

namespace Conway.Rules
{
    public abstract class RuleBase : ScriptableObject
	{
		public bool ApplyOnlyOnFilter = false;
		public ECellType Filter = ECellType.Alive;
		public Vector2Int FilterNeighborhood = Vector2Int.zero;

		public abstract ECellType Apply(Board b, int x, int y);

		public ECellType ApplyWithFilter(Board board, int x, int y)
		{
			if (!ApplyOnlyOnFilter)
				return Apply(board, x, y);

			return ApplyOnType(
				board, 
				x, y, 
				FilterNeighborhood.x, 
				FilterNeighborhood.y
			);
		}

		/*
			Applies the rule only if the current cell is of the Filter type.
			Additionaly, applies the rule to a neighborhood of (nw, nh) around
			the cell too.
		*/
		public ECellType ApplyOnType(Board b, int x, int y, int nw, int nh)
		{
			Conway.ECellType type = b.GetCellCurrent(x, y);
			if (type != Filter)
				return type;

			Conway.ECellType value = Apply(b, x, y);
			b.SetCellCurrent(x, y, value);
			for (int xi = -nw; xi < nw; xi++)
			{
				for (int yi = -nh; yi < nh; yi++)
				{
					if (xi == 0 && yi == 0)
						continue;

					int xx = x+xi;
					int yy = y+yi;

					b.SetCellCurrent(xx, yy, Apply(b, xx, yy));
				}
			}

			return value;
		}
	}
}
