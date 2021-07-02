using System;
using System.Collections;
using System.Collections.Generic;
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
			Texture.filterMode = FilterMode.Point;
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

	[RequireComponent(typeof(BoardComponent))]
	public class PainterComponent : MonoBehaviour
	{
		public PainterConfigurationBase Config;
		public Painter Painter { get; private set; }

		BoardComponent _boardComponent;

		void Awake()
		{
			_boardComponent = GetComponent<BoardComponent>();
			if (_boardComponent == null) Destroy(this);

			_boardComponent.OnBoardGenerated += Cb_OnBoardGenerated;
		}

		void Cb_OnBoardGenerated(BoardComponent.OnBoardGeneratedParams p)
		{
			Painter = new Painter(p.Board, Config);
			p.Board.OnStep += Cb_OnStep;

			MessageRouter.RaiseMessage(new Messages.Painter.OnPainterCreated(this));
		}

		void Cb_OnStep(Conway.Board b)
		{
			Painter.Step(b);
			MessageRouter.RaiseMessage(new Messages.Painter.OnPainterUpdated(this));
		}
	}
}