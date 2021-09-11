

namespace UI
{
    public class UIPopup : UIWindow
    {
        public class Button
        {
            public int Icon;
            public System.Action Callback;
        }

        public UIButtonShader Icon;
        public UIButtonShader[] Buttons;
        private UIVisible _visible;

        public void SetInfo(int icon, UIPopup.Button[] buttons)
        {
            Icon.ButtonType = icon;
            for (int i = 0; i < Buttons.Length; i++)
            {
                var button = Buttons[i];
                if (i < buttons.Length)
                {
                    button.gameObject.SetActive(true);
                    button.SetButtonInfo(buttons[i]);
                }
                else
                {
                    button.gameObject.SetActive(false);
                }
            }

        }
    }
}