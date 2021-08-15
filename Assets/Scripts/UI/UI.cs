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

	public enum EUIState
	{
		MainMenu,
		LevelBrowser,
		LevelInfo,
		Gameplay
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
			Popup.gameObject.GetComponent<Animator>().SetTrigger("Show");
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
			Popup.gameObject.GetComponent<Animator>().SetTrigger("Show");
		}

		public void BtnClick_Play()
		{
			MessageRouter.RaiseMessage(new Messages.UI.OnPlayButtonClick());
		}

		public void BtnClick_StepForward()
		{
			MessageRouter.RaiseMessage(new Messages.UI.OnStepButtonClick(0));
		}

		public void BtnClick_Reset()
		{
			MessageRouter.RaiseMessage(new Messages.UI.OnResetButtonClick());
			MessageRouter.RaiseMessage(new Messages.Gameplay.ResetLevel());
		}

		private void BtnClick_NextLevel()
		{
			Debug.Log("Next level");
			var levelLoader = Conway.LevelLoader.Instance;
			Conway.Data.Level nextLevel;
			if (levelLoader.LevelCollection.NextLevel(levelLoader.Level, out nextLevel))
			{
				MessageRouter.RaiseMessage(new Messages.Gameplay.LoadLevel {
					Level = nextLevel
				});
			}
		}

		private void BtnClick_Replay()
		{
			MessageRouter.RaiseMessage(new Messages.Gameplay.ResetLevel());
		}

		public void BtnClick_Exit()
		{
			Debug.Log("Exit");
		}
	}
}
