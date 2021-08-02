using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIButtonShader : MonoBehaviour
{
    public int ButtonType;
    private int _LastButtonType = -1;
    
    UnityEngine.UI.Image _image;
    Material _material;

    // Start is called before the first frame update
    void OnEnable()
    {
        _image = GetComponent<UnityEngine.UI.Image>();
        _image.material = new Material(_image.material);
        _material = _image.material;
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
        _material.SetInt("_Selected", v ? 1 : 0);
    }
}
