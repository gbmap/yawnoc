using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellType = Conway.ECellType;

/*
	Level building logic. Regular gameplay uses a less resourceful builder,
	while the actual level builder holds infinite resources for all cell types.
*/
namespace Level
{
	public class BuildResource : ScriptableObject
	{
		public ECellType Type;
		public int Count;
	}

	public class Builder 
	{
		public List<BuildResource> Resources;
	}
}
