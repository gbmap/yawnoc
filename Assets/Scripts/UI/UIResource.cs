using System;
using UnityEngine;
using Frictionless;
using Conway.Builder;

namespace UI
{
	[RequireComponent(typeof(UnityEngine.UI.Toggle))]
	[RequireComponent(typeof(UIButtonShader))]
	public class UIResource : MonoBehaviour
	{
		UIButtonShader _btnShader;

		public Conway.ECellType Type { get { return _resource.Type; } }

		public UnityEngine.UI.Toggle Toggle	{ get; private set;	}

		private UnityEngine.UI.Text _text;
		public UnityEngine.UI.Image Icon;

		private Conway.Builder.BuildResource _resource;
		public Conway.Builder.BuildResource Resource 
		{ 
			get { return _resource; } 
		}

		public Conway.Config.BoardStyle Style
		{
			get; set;
		}

		public void SetResource(
			Conway.Builder.BuildResource resource)
		{
			_resource   = resource;
			Icon.color  = Style.GetColor(resource.Type); 
			SetAmount(resource.Count);
		}

		public void SetAmount(int count)
		{
			_text.text = count.ToString();
		}

		public void SetIsSelected(bool value)
		{
			_btnShader?.SetIsSelected(value);
		}

		void Awake()
		{
			Toggle = GetComponent<UnityEngine.UI.Toggle>();
			_text  = GetComponentInChildren<UnityEngine.UI.Text>();
			_btnShader = GetComponent<UIButtonShader>();
		}

		public void OnValueChanged(bool v)
		{
			SetIsSelected(v);
			if (!v) 
				return;

			MessageRouter.RaiseMessage(new Messages.Command.SelectResource
			{
				Resource = Type
			});
		}
	}
}
