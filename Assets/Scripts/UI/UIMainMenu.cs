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
            // Level loader
            MessageRouter.RaiseMessage(new Messages.Command.Play {
                IsPlaying = true
            });

            LevelLoader?.Show(0.75f);
        }
        
        public void Cb_OnHelpClick()
        {
            // 
        }
    }
}