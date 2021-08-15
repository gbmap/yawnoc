using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class DebugUI : MonoBehaviour
    {
        UIWindow _window;

        // Start is called before the first frame update
        void Start()
        {
            _window = FindObjectOfType<UIWindow>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.C))
                _window.Hide();
            else if (Input.GetKeyUp(KeyCode.O))
                _window.Show();
        }
    }
}