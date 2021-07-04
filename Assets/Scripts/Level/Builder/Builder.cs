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
	[System.Serializable]
	public class BuildResource 
	{
		public ECellType Type;
		public int Count;
	}

	public class BuildResources : List<BuildResource> 
	{
		public BuildResources(List<BuildResource> resources)
			: base(resources)
		{}
	}

	public class Builder 
	{
		public List<BuildResource> Resources;
	}
}
