using System.Collections.Generic;
using UnityEngine;
using Conway.Builder;
using UIGame = UI;

namespace Messages
{
    namespace Command
    {
        public class SelectCell
		{
			public Vector2Int Position;
			public Conway.ECellType Type;
		}

		public class PutCell
		{
			public Vector2Int Position;
			public Conway.ECellType Type;
		}

		public class PutCellWorld
		{
			public Vector3 WorldPosition;
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

		public class AddCameraZoomAcceleration
		{
			public float Value;
		}

		public class SelectResource
		{
			public Conway.ECellType Resource;
		}
	}
	
	namespace Input
    {
        public class OnClick
		{
			public Vector3 ScreenPosition;
			public Vector3 WorldPosition;
		}

		public class OnClickUpdate
		{
			public Vector3 StartPosition;
			public Vector3 StartWorldPosition;

			public Vector3 Position;
			public Vector3 WorldPosition;
		}

		public class OnHold
		{
			public Vector3 Position;
			public Vector3 WorldPosition;
			public Vector3 Delta;
		}

		public class OnSwipe
		{
			public Vector3 StartPosition;
			public Vector3 EndPosition;
			public Vector3 Delta;
		}

		public class OnPinch
		{
			public float DeltaDistance;
		}
	}

	namespace Builder
    {
        public class OnBuilderResourcesDepleted { }

		public class OnBuilderResourcesCreated
		{
			public List<BuildResource> Resources;
		}

		public class OnBuilderResourceUpdated
		{
			public BuildResource Resource;
		}
	}

	namespace Board
    {
        public class OnBoardGenerated
		{
			public BoardComponent Component;
			public Conway.Board Board;
		}

		public class OnStep
		{
			public Conway.Board Board;
		}

		public class OnCellPlaced
		{
			public Conway.ECellType Cell;
		}

		public class OnCollectibleObtained
		{
			public Conway.ECellType Cell;
		}

		public class OnCellChanged
		{
			public Vector2Int Position;
			public Conway.ECellType OldType;
			public Conway.ECellType NewType;
		}
	}

	namespace Gameplay
    {
        public class OnGameWon {}
		public class OnGameLost {}

		public class LoadLevel
		{
			public Conway.Data.Level Level;
		}

		public class ResetLevel
		{
		}
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

		public class OnResetButtonClick
		{

		}

		public class OnResourceSelected
		{
			public Conway.ECellType Type;
		}

		public class ShowLevelPopup
		{
			public Conway.Data.Level Level;
		}

		public class OnUIChangeState
		{
			public UIGame.EUIState State;
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
}
