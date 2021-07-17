using UnityEngine;
using Frictionless;

namespace Conway.Rules
{
    [CreateAssetMenu(menuName ="Conway/Rule/Rule Collectible", fileName ="RuleCollectible")]
	public class RuleCollectible : RuleBase
	{
		public ECellType Type = ECellType.Collectible;
		public ECellType Collector = ECellType.Alive;
        public ECellType Reward = ECellType.Alive;

		public override ECellType Apply(Board b, int x, int y)
		{
            if (Reward == ECellType.Dead) 
                return b.GetCellCurrent(new Vector2Int(x, y));

			Vector2Int p = new Vector2Int(x, y);
			if (b.GetCellPrevious(p) == Type &&
				b.GetCellCurrent(p) == Collector)
			{
				MessageRouter.RaiseMessage(
					new Messages.Gameplay.OnCollectibleObtained {
						Cell = Reward
				});
			}

			return b.GetCellCurrent(new Vector2Int(x, y));
		}
	}
}