using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frictionless;
using Input = InputWrapper.Input;

public class CameraComponent : MonoBehaviour
{
	public bool AdjustZoom = true;

	public Vector3 Acceleration;

	[Range(0f, 1f)]
	public float AccelerationDamping = 0.35f;

	public Vector3 Velocity;

	[Range(0f, 1f)]
	public float VelocityDamping = 0.35f;

	void OnEnable()
	{
		MessageRouter.AddHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
		MessageRouter.AddHandler<Messages.Command.SetCameraAcceleration>(Cb_SetCameraAcceleration);
	}

	void OnDisable()
	{
		MessageRouter.RemoveHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
		MessageRouter.RemoveHandler<Messages.Command.SetCameraAcceleration>(Cb_SetCameraAcceleration);
	}

	private void Cb_OnBoardGenerated(Messages.Board.OnBoardGenerated msg)
	{
		Vector3 newPos = msg.Component.GetPosition(msg.Board.Size/2);
		newPos.z = transform.position.z;
		transform.position = newPos;

		if (!AdjustZoom) 
			return;

		GetComponent<Camera>().orthographicSize = msg.Board.Size.y;
	}

	private void Cb_SetCameraAcceleration(Messages.Command.SetCameraAcceleration msg)
	{
		Debug.Log(msg.Acceleration);
		//Velocity = msg.Velocity;
		Acceleration = msg.Acceleration;
	}

    // Update is called once per frame
	void Update()
    {
		Velocity += Acceleration;
		Velocity *= 1f - VelocityDamping;
		Acceleration *= 1f - AccelerationDamping;

		transform.position += Velocity * Time.deltaTime;

		GetComponent<Camera>().orthographicSize -= Input.mouseScrollDelta.y;

		if (Input.touchCount == 0)
			return;

		Touch touch = Input.GetTouch(0);
		Debug.Log("!!!");

		Vector3 acc = -touch.deltaPosition;

		// Don't think about it
		MessageRouter.RaiseMessage(new Messages.Command.SetCameraAcceleration
		{
			Acceleration = acc
		});
    }

}
