using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conway.Rules
{
	[CreateAssetMenu(menuName="Conway/Rules/Ruleset", fileName="Ruleset")]
	public class Ruleset : ScriptableObject
	{
		public RuleBase[] Rules;

		public void Apply(Conway.Board b)
		{
			foreach (RuleBase r in Rules)
				b.ApplyRule(r);
		}
	}
}
