using UnityEngine;
using Frictionless;
using Input = InputWrapper.Input;
using Messages.Input;
using System;
using Messages.UI;

[RequireComponent(typeof(Camera))]
public class CameraComponent : MonoBehaviour
{
	public bool AdjustZoom = true;

	public Vector3 Acceleration;

	[Range(0f, 1f)]
	public float AccelerationDamping = 0.35f;

	public Vector3 Velocity;

	[Range(0f, 1f)]
	public float VelocityDamping = 0.35f;

	// public float Zoom;

	[Range(0f, 1f)]
	public float ZoomAcceleration;

	[Range(0f, 1f)]
	public float ZoomVelocity;

	[Range(0f, 1f)]
	public float ZoomDamping = 0.35f;

	Camera _camera;

	private bool _isOnGameplay;

	void Awake()
	{
		_camera = GetComponent<Camera>();
	}

	void OnEnable()
	{
		MessageRouter.AddHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
		MessageRouter.AddHandler<Messages.Command.SetCameraAcceleration>(Cb_SetCameraAcceleration);
		MessageRouter.AddHandler<Messages.Input.OnPinch>(Cb_OnPinch);
		MessageRouter.AddHandler<Messages.Input.OnClickUpdate>(Cb_OnTouchUpdate);
		MessageRouter.AddHandler<Messages.UI.OnChangeState>(Cb_OnChangeState);
	}

	void OnDisable()
	{
		MessageRouter.RemoveHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
		MessageRouter.RemoveHandler<Messages.Command.SetCameraAcceleration>(Cb_SetCameraAcceleration);
		MessageRouter.RemoveHandler<Messages.Input.OnPinch>(Cb_OnPinch);
		MessageRouter.RemoveHandler<Messages.Input.OnClickUpdate>(Cb_OnTouchUpdate);
		MessageRouter.RemoveHandler<Messages.UI.OnChangeState>(Cb_OnChangeState);
	}

    private void Cb_OnChangeState(OnChangeState obj)
    {
		_isOnGameplay = obj.State == UI.EUIState.Gameplay;
    }

    private void Cb_OnBoardGenerated(Messages.Board.OnBoardGenerated msg)
	{
		Vector3 newPos = msg.Component.GetPosition(msg.Board.Size/2);
		newPos.z = transform.position.z;
		transform.position = newPos;

		if (!AdjustZoom) 
			return;

		_camera.orthographicSize = msg.Board.Size.y;
	}

	private void Cb_SetCameraAcceleration(Messages.Command.SetCameraAcceleration msg)
	{
		Acceleration = msg.Acceleration;
	}

    // Update is called once per frame
	void Update()
    {
		Velocity += Acceleration;
		Velocity *= 1f - VelocityDamping;
		Acceleration *= 1f - AccelerationDamping;

		transform.position += Velocity * Time.deltaTime;

		ZoomAcceleration -= (Input.mouseScrollDelta.y)
						   *(Mathf.Abs(Input.mouseScrollDelta.y))*2f*_camera.orthographicSize;

		ZoomVelocity 	 += ZoomAcceleration;
		ZoomVelocity 	 *= 1f - ZoomDamping;
		ZoomAcceleration *= 1f - ZoomDamping;
		_camera.orthographicSize += ZoomVelocity * Time.deltaTime;
		// Debug.Log(_camera.orthographicSize);

    }

    private void Cb_OnPinch(OnPinch obj)
    {
		Debug.Log("Pinch.");
		ZoomAcceleration = obj.DeltaDistance;
    }

    private void Cb_OnTouchUpdate(OnClickUpdate msg)
    {
		if (!_isOnGameplay) return;
		Acceleration = -msg.DeltaPosition * _camera.orthographicSize;
    }
}
