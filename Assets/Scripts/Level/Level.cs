using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BuildResource = Level.BuildResource;

namespace Data
{
	[CreateAssetMenu(menuName="CGOD/Level", fileName="Level")]
	public class Level : ScriptableObject
	{
		public Gameplay.EMode Mode;
		public Texture2D Texture;
		public List<BuildResource> Resources;
	}
}
