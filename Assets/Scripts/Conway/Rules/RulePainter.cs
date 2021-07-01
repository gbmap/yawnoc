using UnityEngine;

namespace Conway.Rules
{
	public class Painter : RuleBase
	{
		private Vector2Int _size;
		private Color[,] _texture;
		public Painter(Vector2Int size)
		{
			_size = size;
		}

		public override ECellType Apply(Board b, int x, int y)
		{
			return ECellType.Dead;
		}
	}
}
