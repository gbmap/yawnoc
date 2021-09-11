using Frictionless;
using Messages.UI;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(UILevelInfo))]
    public class UILevelInfoWindow : UIWindow
    {
        UILevelInfo _levelInfo;

        protected override void Awake()
        {
            base.Awake();
            _levelInfo = gameObject.GetComponent<UILevelInfo>();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            MessageRouter.AddHandler<Messages.UI.ShowLevelPopup>(Cb_ShowLevelPopup);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            MessageRouter.RemoveHandler<Messages.UI.ShowLevelPopup>(Cb_ShowLevelPopup);
        }

        private void Cb_ShowLevelPopup(ShowLevelPopup msg)
        {
            _levelInfo.Level = msg.Level;
            MessageRouter.RaiseMessage(new Messages.UI.OnChangeState {
                State = EUIState.LevelInfo
            });
        }

        public void Cb_PlayClick()
        {
            MessageRouter.RaiseMessage(new Messages.Gameplay.LoadLevel {
                Level = _levelInfo.Level,
                UpdateUIState = true
            });
        }
    }
}