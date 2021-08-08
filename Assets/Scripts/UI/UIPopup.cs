using UnityEngine;

public class UIPopup : MonoBehaviour
{
    public class Button
    {
        public int Icon;
        public System.Action Callback;
    }

    public UIButtonShader Icon;
    public UIButtonShader[] Buttons;

    public void Show(int icon, Button[] buttons)
    {
        Icon.ButtonType = icon;
        for (int i = 0; i < Mathf.Min(Buttons.Length, buttons.Length); i++)
        {
            var button = Buttons[i];
            button.SetButtonInfo(buttons[i]);
        }
    }
}
