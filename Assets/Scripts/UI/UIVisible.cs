using System.Collections.Generic;
using UnityEngine;

namespace UI
{
    public class UIVisible : MonoBehaviour
    {
        UIButtonShader _shader;
        List<Component> _components;
        float _target;

        System.Type[] _types = {
//          typeof(UnityEngine.UI.Image),
            typeof(UnityEngine.UI.Text),
            typeof(UnityEngine.UI.Selectable)
        };

        void Awake()
        {
            _shader = GetComponent<UIButtonShader>();
            _components = new List<Component>();
            for (int i = 0; i < _types.Length; i++) {
                System.Type type = _types[i];
                _components.AddRange(GetComponents(type));
            }

            Show();
        }

        void FixedUpdate()
        {
            if (!_shader)
                return;

            _shader.DisappearAnimation = 
                Mathf.Clamp01(_shader.DisappearAnimation + Mathf.Sign(_target) * Time.deltaTime*2f);
        }

        public void Show()
        {
            _target = -1f;
            foreach (var comp in _components)
            {
                if (comp is MonoBehaviour)
                    (comp as MonoBehaviour).enabled = true;
            }
        }

        public void Hide()
        {
            _target = 1f;
            foreach (var comp in _components)
            {
                if (comp is MonoBehaviour)
                    (comp as MonoBehaviour).enabled = false;
            }
        }
    }
}
