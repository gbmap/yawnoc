using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frictionless;

public class MoveToCenterOfBoard : MonoBehaviour
{
	public bool AdjustZoom = true;
    void OnEnable()
    {
		MessageRouter.AddHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
    }

    void OnDisable()
    {
		MessageRouter.RemoveHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
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
}
