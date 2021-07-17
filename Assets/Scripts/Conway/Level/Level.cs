using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuildResource = Conway.Builder.BuildResource;

namespace Conway.Data
{
	public abstract class Level : ScriptableObject
	{
		public ELevelMode Mode;
		public List<BuildResource> Resources;

		public abstract Conway.Board Load(Conway.Config.BoardStyle style);
	}
}
