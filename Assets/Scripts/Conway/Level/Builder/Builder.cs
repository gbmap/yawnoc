using System.Collections.Generic;
using System.Linq;


/*
	Level building logic. Regular gameplay uses a less resourceful builder,
	while the actual level builder holds infinite resources for all cell types.
*/
namespace Conway.Builder
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
			: base()
		{
			// Create local copy of resources to prevent overwriting
			// scriptable objects.
			foreach(var resource in resources)
			{
				Add(new BuildResource {
					Type  = resource.Type,
					Count = resource.Count
				});
			}
		}

		public BuildResource Get(ECellType cell)
		{
			var resource = this.FirstOrDefault(x=>x.Type == cell);
			if (resource == null)
			{
				return new BuildResource {
					Type = cell,
					Count = 0
				};
			}

			return resource;
		}

		public bool HasResources
		{
			get { return this.Any(x=>x.Count > 0); }
		}
	}

	public class Builder
	{
		public List<BuildResource> Resources;
	}
}
