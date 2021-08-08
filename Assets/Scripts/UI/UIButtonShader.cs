using UnityEngine;

public class UIButtonShader : MonoBehaviour
{
    public int ButtonType;
    private int _LastButtonType = -1;
    
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

        _button = GetComponent<UnityEngine.UI.Button>();
        _button?.onClick.AddListener(Cb_OnClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (ButtonType != _LastButtonType)
        {
            _material.SetInt("_Button", ButtonType);
            _LastButtonType = ButtonType;
        }
    }

    public void SetIsSelected(bool v)
    {
        _material?.SetInt("_Selected", v ? 1 : 0);
    }

    public void SetButtonInfo(UIPopup.Button button)
    {
        _callback = button.Callback;
    }
    
    private void Cb_OnClick()
    {
        _callback?.Invoke();
    }
}
