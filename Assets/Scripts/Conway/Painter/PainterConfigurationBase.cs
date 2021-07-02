using UnityEngine;

namespace Conway
{
	public abstract class PainterConfigurationBase : ScriptableObject
	{
		public int StepsToMax = 32;
		public abstract Color Sample(float t);
	}

	[CreateAssetMenu(menuName="Conway/Painter/Brush Gradient", fileName="PainterConfigurationGradient")]
	public class PainterConfigurationGradient : PainterConfigurationBase
	{
		public Gradient Gradient;

		public override Color Sample(float t)
		{
			return Gradient.Evaluate(t);
		}
	}

}
