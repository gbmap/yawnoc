using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frictionless;

namespace UI 
{
	public class UI : MonoBehaviour
	{
		void OnEnable()
		{
			MessageRouter.AddHandler<Messages.Gameplay.OnGameWon>(Cb_OnGameWon);
		}

		void OnDisable()
		{
			MessageRouter.RemoveHandler<Messages.Gameplay.OnGameWon>(Cb_OnGameWon);
		}

		public void Cb_OnGameWon(Messages.Gameplay.OnGameWon msg)
		{
			Debug.Log("WON");
		}

		public void BtnClick_Play()
		{
			MessageRouter.RaiseMessage(new Messages.UI.OnPlayButtonClick());
		}

		public void BtnClick_StepForward()
		{
			MessageRouter.RaiseMessage(new Messages.UI.OnStepButtonClick(0));
		}

		void DrawGizmos()
		{
			RectTransform t = GetComponent<RectTransform>();
			//Gizmos.DrawWireCube(
			//t.rect
		}
	}
}
