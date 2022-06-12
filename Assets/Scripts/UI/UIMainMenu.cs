using System.Collections;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace UI
{
    public class UIMainMenu : MonoBehaviour
    {
        public UIWindow LevelLoader;

        Coroutine _coroutine;
        
        public void Cb_OnPlayClick()
        {
            if (_coroutine != null)
                return;

            _coroutine = StartCoroutine(Coroutine_Play());
        }

        IEnumerator Coroutine_Play()
        {
            MessageRouter.RaiseMessage(new Messages.Command.Play { IsPlaying = true });

            yield return new WaitForSeconds(2f);

            MessageRouter.RaiseMessage(
                new Messages.UI.OnChangeState {
                    State = EUIState.LevelBrowser
                }
            );

            _coroutine = null;
        }
        
        public void Cb_OnHelpClick()
        {
            // 
        }
    }
}