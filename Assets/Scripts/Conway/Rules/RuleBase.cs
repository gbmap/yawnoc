using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Conway.Rules
{
	public abstract class RuleBase : ScriptableObject
	{
		public abstract ECellType Apply(Board b, int x, int y);
	}
}
