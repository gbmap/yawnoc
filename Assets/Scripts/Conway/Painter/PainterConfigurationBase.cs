using UnityEngine;

namespace Conway
{
	public abstract class PainterConfigurationBase : ScriptableObject
	{
		public int StepsToMax = 32;
		public abstract Color Sample(float t);
	}
}
