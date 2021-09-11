using UnityEngine;
using Frictionless;
using Input = InputWrapper.Input;

namespace Controls
{
	public enum EScreenArea
	{
		Menu,
		Board
	}

	public class TouchController : MonoBehaviour
	{
		float   clickStartTime;
		float   clickEndTime;
		Vector2 startPosition;
		Vector2 endPosition;

		bool lastClickState;
		bool hasFiredHoldEvent;

		float _lastZoomDistance;

		UI.UIRoot _uiRoot;

		void OnEnable()
		{
			_uiRoot = FindObjectOfType<UI.UIRoot>();
		}
		
		void Update()
		{
			if (Input.GetKey(KeyCode.P))
			{
				MessageRouter.RaiseMessage(new Messages.Input.OnPinch
				{
					DeltaDistance = 10f * Time.deltaTime
				});
			}

			if (Input.touchCount == 0)
			{
				if (lastClickState)
					StopTouch();
				return;
			}

			if (CheckZoom())
				return;

			Touch touch = Input.GetTouch(0);
			if (!lastClickState)
				StartTouch(touch);
			else
				UpdateTouch(touch);

			lastClickState = true;
		}

		bool CheckZoom()
		{
			if (Input.touchCount < 2)
			{
				_lastZoomDistance = Mathf.NegativeInfinity;
				return false;
			}

			var touchA = Input.GetTouch(0);
			var touchB = Input.GetTouch(1);

			Vector3 posA = Camera.main.ScreenToViewportPoint(touchA.position);
			Vector3 posB = Camera.main.ScreenToViewportPoint(touchB.position);

			float distance = Vector3.Distance(posA, posB);
			float deltaDistance = distance - _lastZoomDistance;
			MessageRouter.RaiseMessage(new Messages.Input.OnPinch
			{
				DeltaDistance = distance * 10f
			});
			return true;
		}

		void StartTouch(Touch touch)
		{
			startPosition     = touch.position;
			clickStartTime    = Time.time;
			hasFiredHoldEvent = false;
		}

		void UpdateTouch(Touch touch)
		{
			float clickTime = Mathf.Abs(Time.time - clickStartTime); 
			endPosition = touch.position;

			if (clickTime > 1f && !hasFiredHoldEvent)
			{
				OnHold(touch.position, touch.position - startPosition);
				hasFiredHoldEvent = true;
			}

			OnTouchUpdate(startPosition, touch.position, touch.deltaPosition);
		}

		void StopTouch()
		{
			lastClickState    = false;
			clickEndTime      = Time.time;
			hasFiredHoldEvent = false;

			float clickTime = Mathf.Abs(clickStartTime - clickEndTime); 
			Vector3 delta   = endPosition - startPosition;

			if (delta.sqrMagnitude < 0.75f)
			{
				if (clickTime <= 1.20f)
					OnClick(startPosition);
			}
		}

		void OnClick(Vector3 position)
		{
			MessageRouter.RaiseMessage(new Messages.Input.OnClick
			{
				ScreenPosition = position,
				WorldPosition  = Camera.main.ScreenToWorldPoint(position),
				IsClickOnUI    = _uiRoot ? _uiRoot.IsTouchOnUI : false
			});
		}

		void OnTouchUpdate(
			Vector3 startPosition, 
			Vector3 position,
			Vector3 deltaPosition
		) {
			MessageRouter.RaiseMessage(new Messages.Input.OnClickUpdate
			{
				StartPosition      = startPosition,
				StartWorldPosition = Camera.main.ScreenToWorldPoint(startPosition),
				Position           = position,
				WorldPosition      = Camera.main.ScreenToWorldPoint(position),
				DeltaPosition      = deltaPosition,
				WorldDeltaPosition = Camera.main.ScreenToWorldPoint(deltaPosition)
			});
		}

		void OnHold(Vector3 position, Vector3 delta)
		{
			MessageRouter.RaiseMessage(new Messages.Input.OnHold
			{
				Position = position,
				WorldPosition = Camera.main.ScreenToWorldPoint(position),
				Delta = delta
			});
		}

		void OnGUI()
		{
			GUILayout.Label($"{Input.touchCount}");
			GUILayout.Label($"{clickStartTime}");
			GUILayout.Label($"{clickEndTime}");
			GUILayout.Label($"{startPosition}");
			GUILayout.Label($"{endPosition}");
			GUILayout.Label($"{lastClickState}");
			GUILayout.Label($"{hasFiredHoldEvent}");
			if (_uiRoot)
				GUILayout.Label($"{_uiRoot.IsTouchOnUI}");
		}
	}
}
