using UnityEngine;
using Frictionless;
using Messages.Command;
using Messages.Gameplay;

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
		public GameObject WinScreen;

		void OnEnable()
		{
			MessageRouter.AddHandler<Messages.Gameplay.OnGameWon>(Cb_OnGameWon);
			MessageRouter.AddHandler<Messages.Gameplay.OnGameLost>(Cb_OnGameLost);
			MessageRouter.AddHandler<Messages.Command.Play>(Cb_OnPlay);
		}

		void OnDisable()
		{
			MessageRouter.RemoveHandler<Messages.Gameplay.OnGameWon>(Cb_OnGameWon);
			MessageRouter.AddHandler<Messages.Gameplay.OnGameLost>(Cb_OnGameLost);
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
			if (!WinScreen)
				return;

			WinScreen.SetActive(true);
		}

        private void Cb_OnGameLost(OnGameLost obj)
        {

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
		}
	}
}
