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
                return b.CurrentState.Get(x, y);

			if (b.PreviousState.Get(x, y) == Type &&
				b.CurrentState.Get(x, y) == Collector)
			{
				MessageRouter.RaiseMessage(
					new Messages.Gameplay.OnCollectibleObtained {
						Cell = Reward
				});
			}

			return b.CurrentState.Get(x, y);
		}
	}
}