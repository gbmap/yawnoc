using UnityEngine;

namespace Conway.Data
{
    [CreateAssetMenu(menuName="Conway/Level/Texture Level", fileName="TextureLevel")]
	public class LevelTexture : Level
	{
		public Texture2D Texture;

		public override Conway.Board Load(Config.BoardStyle style)
		{
			Vector2Int size = new Vector2Int(Texture.width, Texture.height);
			Conway.Board b = new Conway.Board(size, null, style);

			for (int x = 0; x < Texture.width; x++)
			{
				for (int y = 0; y < Texture.height; y++)
				{
					Color clr = Texture.GetPixel(x, y);
					b.SetCell(new Vector2Int(x, y), b.Style.GetType(clr));
				}
			}
			return b;
		}
	}
}