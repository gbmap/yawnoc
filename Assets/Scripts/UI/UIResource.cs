using System;
using UnityEngine;
using Frictionless;

namespace UI
{
	[RequireComponent(typeof(UnityEngine.UI.Toggle))]
	public class UIResource : MonoBehaviour
	{
		public Conway.ECellType Type { get { return _resource.Type; } }

		public UnityEngine.UI.Toggle Toggle	{ get; private set;	}

		private UnityEngine.UI.Text _text;
		public UnityEngine.UI.Image Icon;

		private Level.BuildResource _resource;
		public Level.BuildResource Resource 
		{ 
			get { return _resource; } 
		}

		public void SetResource(Level.BuildResource resource)
		{
			_resource   = resource;
			_text.text  = resource.Count.ToString();
			Icon.color = Gameplay.BoardLoader.CellToColor(resource.Type);
		}

		void Awake()
		{
			Toggle = GetComponent<UnityEngine.UI.Toggle>();
			_text  = GetComponentInChildren<UnityEngine.UI.Text>();
		}

		public void OnValueChanged(bool v)
		{
			if (!v) 
				return;

			MessageRouter.RaiseMessage(new Messages.Command.SelectResource
			{
				Resource = Type
			});
		}
	}
}
