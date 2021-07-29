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
			Debug.Log("Start");
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

			OnClickUpdate(startPosition, touch.position);
		}

		void StopTouch()
		{
			lastClickState    = false;
			clickEndTime      = Time.time;
			endPosition       = endPosition;
			hasFiredHoldEvent = false;

			Debug.Log("Stop.");
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
				WorldPosition = Camera.main.ScreenToWorldPoint(position)
			});
			Debug.Log("Click");
		}

		void OnClickUpdate(Vector3 startPosition, Vector3 position)
		{
			MessageRouter.RaiseMessage(new Messages.Input.OnClickUpdate
			{
				StartPosition      = startPosition,
				StartWorldPosition = Camera.main.ScreenToWorldPoint(startPosition),
				Position           = position,
				WorldPosition      = Camera.main.ScreenToWorldPoint(position)
			});
			Debug.Log("Click Update");
		}

		void OnHold(Vector3 position, Vector3 delta)
		{
			MessageRouter.RaiseMessage(new Messages.Input.OnHold
			{
				Position = position,
				WorldPosition = Camera.main.ScreenToWorldPoint(position),
				Delta = delta
			});
			Debug.Log("Hold");
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
		}
	}
}
