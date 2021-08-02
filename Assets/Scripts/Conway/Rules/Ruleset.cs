using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

namespace Conway.Rules
{
	[CreateAssetMenu(menuName="Conway/Rules/Ruleset", fileName="Ruleset")]
	public class Ruleset : ScriptableObject
	{
		public RuleBase[] Rules;

		public void Apply(Conway.Board b)
		{
			b.ForEachCell(delegate (Board.ForEachCellParams p)
			{
				foreach (RuleBase r in Rules)
				{
					{
						b.ApplyRule(r, p.Position.x, p.Position.y);
					}
				}
			});
		}
	}
}
