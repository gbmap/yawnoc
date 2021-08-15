using System.Collections;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace UI
{
    public class UIMainMenu : MonoBehaviour
    {
        public UIWindow LevelLoader;

        
        public void Cb_OnPlayClick()
        {
            MessageRouter.RaiseMessage(new Messages.UI.OnUIChangeState {
                State = EUIState.LevelBrowser
            });
        }
        
        public void Cb_OnHelpClick()
        {
            // 
        }
    }
}