using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conway.Rules
{
	[CreateAssetMenu(menuName="Conway/Rules/Maintain Cell", fileName="MaintainCell")]
	public class RuleMaintainCell : RuleBase
	{
		public ECellType Value;

		public override ECellType Apply(Board b, int cx, int cy)
		{
			if (b.PreviousState.Get(cx, cy) == Value)
				return Value;
			return b.CurrentState.Get(cx, cy);
		}
	}
}
