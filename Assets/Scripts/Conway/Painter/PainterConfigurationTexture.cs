using UnityEngine;

namespace Conway
{
	[CreateAssetMenu(menuName="Conway/Painter/Brush Texture", fileName="PainterConfigurationTexture")]
	public class PainterConfigurationTexture : PainterConfigurationBase
	{
		public Texture2D Gradient;

		public override Color Sample(float t)
		{
			t = Mathf.Clamp01(t);
			int x = Mathf.RoundToInt(t * Gradient.width);
			return Gradient.GetPixel(x, 0);
		}
	}
}
