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

        protected virtual void OnEnable()
        {
            MessageRouter.AddHandler<Messages.UI.ShowLevelPopup>(Cb_ShowLevelPopup);
        }

        void OnDisable()
        {
            // MessageRouter.RemoveHandler<Messages.UI.ShowLevelPopup>(Cb_ShowLevelPopup);
        }

        private void Cb_ShowLevelPopup(ShowLevelPopup obj)
        {
            Debug.Log("Show level info popup.");

            _levelInfo.Level = obj.Level;
            Show(0.1f);
        }
    }
}