using Frictionless;
using UnityEngine;

namespace UI
{
    public class UILevelInfo : MonoBehaviour
    {
        public UnityEngine.UI.Image  Image;
        public UnityEngine.UI.Button Button;

        private Conway.Data.Level _level;
        public Conway.Data.Level Level 
        {
            get { return _level; }
            set
            {
                _level = value;
                if (!(_level is Conway.Data.LevelTexture))
                    return;

                var texture = (_level as Conway.Data.LevelTexture).Texture;
                var spr = Sprite.Create(
                    texture,
                    Rect.MinMaxRect(0f, 0f, texture.width, texture.height),
                    new Vector2(0.5f, 0.5f),
                    100f
                );

                Image.sprite = spr;

                if (!Button)
                    return;

                Button.onClick.RemoveAllListeners();
                Button.onClick.AddListener(Cb_OnInfoClicked);
            }
        }

        void Cb_OnInfoClicked()
        {
            MessageRouter.RaiseMessage(
                new Messages.UI.ShowLevelPopup {
                    Level = this.Level
                }
            );
        }
    }
}