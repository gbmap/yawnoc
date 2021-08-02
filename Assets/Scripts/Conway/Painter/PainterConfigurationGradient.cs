using UnityEngine;

namespace Conway
{
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