using UnityEngine;
using Frictionless;
using Messages.Command;
using Messages.Gameplay;
using System.Collections;

namespace UI
{
    public enum EIcon
	{
		Play,
		Step,
		Pause,
		Exit,
		Replay,
		Win
	}

    public class UI : MonoBehaviour
	{
		public UIButtonShader PlayButtonShader;
		public UIPopup Popup;

		void OnEnable()
		{
			MessageRouter.AddHandler<Messages.Gameplay.OnGameWon>(Cb_OnGameWon);
			MessageRouter.AddHandler<Messages.Gameplay.OnGameLost>(Cb_OnGameLost);
			MessageRouter.AddHandler<Messages.Command.Play>(Cb_OnPlay);
		}

		void OnDisable()
		{
			MessageRouter.RemoveHandler<Messages.Gameplay.OnGameWon>(Cb_OnGameWon);
			MessageRouter.RemoveHandler<Messages.Gameplay.OnGameLost>(Cb_OnGameLost);
			MessageRouter.RemoveHandler<Messages.Command.Play>(Cb_OnPlay);
		}

        private void Cb_OnPlay(Play obj)
        {
			if (!PlayButtonShader)
				return;

			PlayButtonShader.ButtonType = obj.IsPlaying ? 2 : 0;
        }

        public void Cb_OnGameWon(Messages.Gameplay.OnGameWon msg)
		{
			if (!Popup)
				return;
			
			StartCoroutine(Coroutine_Victory());
		}

		IEnumerator Coroutine_Victory()
		{
			yield return new WaitForSeconds(0.75f);
			Popup.Show(5, new UIPopup.Button[] {
				new UIPopup.Button {
					 Icon = 0,
					 Callback = BtnClick_NextLevel 
				},

				new UIPopup.Button {
					Icon = 4,
					Callback = BtnClick_Replay
				},

				new UIPopup.Button {
					Icon = 3,
					Callback = BtnClick_Exit
				}
			});
			Popup.gameObject.SetActive(true);
		}

        private void Cb_OnGameLost(OnGameLost obj)
        {
			if (!Popup)
				return;

			StartCoroutine(Coroutine_Defeat());
        }

		IEnumerator Coroutine_Defeat()
		{
			yield return new WaitForSeconds(0.75f);
			Popup.Show(5, new UIPopup.Button[] {
				new UIPopup.Button {
					Icon = 4,
					Callback = BtnClick_Replay
				},

				new UIPopup.Button {
					Icon = 3,
					Callback = BtnClick_Exit
				}
			});
			Popup.gameObject.SetActive(true);
		}

		public void BtnClick_Play()
		{
			MessageRouter.RaiseMessage(new Messages.UI.OnPlayButtonClick());
		}

		public void BtnClick_StepForward()
		{
			MessageRouter.RaiseMessage(new Messages.UI.OnStepButtonClick(0));
		}

		private void BtnClick_NextLevel()
		{
			Debug.Log("Next level");
		}

		private void BtnClick_Replay()
		{
			Debug.Log("Replay");
		}

		public void BtnClick_Exit()
		{
			Debug.Log("Exit");
		}
	}
}
