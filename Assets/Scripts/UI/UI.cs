using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frictionless;

namespace UI {
	public class UI : MonoBehaviour
	{
		public void BtnClick_Play()
		{
			MessageRouter.RaiseMessage(new Messages.UI.OnPlayButtonClick());
		}
	}
}
