using Frictionless;
using Messages.Gameplay;
using UnityEngine;

namespace UI
{
    public class UIWindowCloseOnLoadLevel : MonoBehaviour
    {
        UIWindow _window;

        void Awake()
        {
            _window = GetComponent<UIWindow>();
        }
        
        void OnEnable()
        {
            MessageRouter.AddHandler<Messages.Gameplay.LoadLevel>(Cb_LoadLevel);
        }

        void OnDisable()
        {
            MessageRouter.RemoveHandler<Messages.Gameplay.LoadLevel>(Cb_LoadLevel);
        }

        private void Cb_LoadLevel(LoadLevel obj)
        {
            _window?.Hide();
        }
    }
}