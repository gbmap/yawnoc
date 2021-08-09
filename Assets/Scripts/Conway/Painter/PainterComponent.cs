using UnityEngine;
using Frictionless;

namespace Conway
{
    [System.Serializable]
	public class PainterConfiguration
	{
		public Gradient Gradient;

		// Max steps active to reach 1.
		public int StepsToMax;
	}

	public class Painter
	{
		public Texture2D Texture { get; private set; }
		private Vector2Int _size;
		private float[,] _values;
		PainterConfigurationBase _cfg;

		private float ValueStep
		{
			get { return 1f / _cfg.StepsToMax; }
		}

		public Painter(Conway.Board b, PainterConfigurationBase cfg)
		{
			_cfg    = cfg;
			_size   = b.Size;
			_values = new float[_size.x, _size.y];
			Texture = new Texture2D(_size.x, _size.y);
			for (int x = 0; x < _size.x; x++)
			{
				for (int y = 0; y < _size.y; y++)
				{
					Texture.SetPixel(x, y, new Color(0f, 0f, 0f, 1f));
				}
			}
			Texture.filterMode = FilterMode.Point;
			Texture.Apply();
		}

		public void Step(Conway.Board b)
		{
			b.ForEachCell(PaintCell);
			Texture.Apply();
		}

		public void PaintCell(Conway.Board.ForEachCellParams p)
		{
			if (p.State == ECellType.Dead) return;
			float v= _values[p.Position.x, p.Position.y]; 
			v = Mathf.Clamp01(v+ValueStep);
			_values[p.Position.x, p.Position.y] = v;
			Color clr = _cfg.Sample(v);
			clr.a = 1f;
			Texture.SetPixel(p.Position.x, p.Position.y, clr);
		}
	}

	public class PainterComponent : MonoBehaviour
	{
		public PainterConfigurationBase Config;
		public Painter Painter { get; private set; }

		BoardComponent _boardComponent;

		void OnEnable()
		{
			MessageRouter.AddHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
			MessageRouter.AddHandler<Messages.Board.OnStep>(Cb_OnStep);
		}

		void OnDisable()
		{
			MessageRouter.RemoveHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
			MessageRouter.RemoveHandler<Messages.Board.OnStep>(Cb_OnStep);
		}

		void Cb_OnBoardGenerated(Messages.Board.OnBoardGenerated msg)
		{
			Painter = new Painter(msg.Board, Config);

			MessageRouter.RaiseMessage(new Messages.Painter.OnPainterCreated(this));
		}

		void Cb_OnStep(Messages.Board.OnStep msg)
		{
			Painter.Step(msg.Board);
			MessageRouter.RaiseMessage(new Messages.Painter.OnPainterUpdated(this));
		}
	}
}
