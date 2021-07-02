using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frictionless;

namespace Messages
{
	namespace Command
	{
		public class PutCell
		{
			public Vector2Int Position;
			public Conway.ECellType Type;
		}

		public class Play
		{
			public bool IsPlaying;
		}

		public class Step
		{
		}

		public class SetStepSpeed
		{
			public float Speed;
		}

		public class SetCameraAcceleration
		{
			public Vector3 Acceleration;
		}
	}

	namespace Board
	{
		public class OnBoardGenerated
		{
			public BoardComponent Component;
			public Conway.Board Board;
		}
	}

	namespace Gameplay
	{
		public class OnGameWon {}
		public class OnGameLost {}
	}

	namespace UI
	{
		public class OnPlayButtonClick 	{}
		public class OnStepButtonClick 
		{
			public int Speed;
			public OnStepButtonClick(int speed)
			{
				Speed = speed;
			}
		}
	}

	namespace Painter
	{
		public class OnPainterCreated 
		{
			public Conway.PainterComponent Painter { get; private set; }
			public OnPainterCreated(Conway.PainterComponent c)
			{
				Painter = c;
			}
		}

		public class OnPainterUpdated 
		{
			public Conway.PainterComponent Painter { get; private set; }
			public OnPainterUpdated(Conway.PainterComponent c)
			{
				Painter = c;
			}
		}
	}

	public class Postman : MonoBehaviour
	{
		void Awake()
		{
		}

		public static Postman Get()
		{
			return FindObjectOfType<Postman>();
		}
	}
}
