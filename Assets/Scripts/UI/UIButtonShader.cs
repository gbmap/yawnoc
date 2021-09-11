using UnityEngine;

namespace UI
{
    public class UIButtonShader : MonoBehaviour
    {
        public int ButtonType;
        private int _LastButtonType = -1;

        [Range(0f, 1f)]
        public float DisappearAnimation;
        private float _lastDisappearT;
        
        UnityEngine.UI.Image  _image;
        UnityEngine.UI.Button _button;
        Material              _material;
        System.Action         _callback;

        // Start is called before the first frame update
        void Awake()
        {
            _image = GetComponent<UnityEngine.UI.Image>();
            _image.material = new Material(_image.material);
            _material = _image.material;
            UpdateIcon();

            _lastDisappearT = float.NegativeInfinity;

            _button = GetComponent<UnityEngine.UI.Button>();
            _button?.onClick.AddListener(Cb_OnClick);
        }

        void OnEnable()
        {
            UpdateIcon();
        }

        // Update is called once per frame
        void Update()
        {
            if (ButtonType != _LastButtonType)
            {
                UpdateIcon();
            }

            if (Mathf.Abs(DisappearAnimation - _lastDisappearT) > float.Epsilon)
            {
                _material.SetFloat("_AnimDisappear", DisappearAnimation);
                _lastDisappearT = DisappearAnimation;
            }
        }

        private void UpdateIcon()
        {
            _material.SetFloat("_Button", ButtonType);
            _LastButtonType = ButtonType;
        }

        public void SetIsSelected(bool v)
        {
            _material?.SetFloat("_Selected", v ? 1 : 0);
        }

        public void SetButtonInfo(UIPopup.Button button)
        {
            _callback = button.Callback;
            ButtonType = button.Icon;
        }
        
        private void Cb_OnClick()
        {
            _callback?.Invoke();
        }
    }
}