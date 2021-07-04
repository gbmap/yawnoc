using System;
using System.Collections.Generic;
using UnityEngine;
using Frictionless;

namespace UI
{
	/*
		Side menu containing cells to be put.
	*/
	[RequireComponent(typeof(UnityEngine.UI.ToggleGroup))]
	public class UIBuilderResources : MonoBehaviour
	{
		public GameObject UIBuilderResourcePrefab;
		private Dictionary<Conway.ECellType, UIResource> _instances;

		void Awake()
		{
			_instances = new Dictionary<Conway.ECellType, UIResource>();		
		}

		void OnEnable()
		{
			MessageRouter.AddHandler<Messages.Builder.OnBuilderResourcesCreated>(Cb_OnResourcesCreated);
		}

		void OnDisable()
		{
			MessageRouter.RemoveHandler<Messages.Builder.OnBuilderResourcesCreated>(Cb_OnResourcesCreated);
		}

		void Cb_OnResourcesCreated(Messages.Builder.OnBuilderResourcesCreated msg)
		{
			SpawnUIResources(msg.Resources);
		}

		void ClearUIResources()
		{
			foreach (UIResource instance in _instances.Values)
				Destroy(instance.gameObject);

			_instances.Clear();
		}

		void SpawnUIResources(List<Level.BuildResource> resources)
		{
			ClearUIResources();

			foreach (Level.BuildResource rsrc in resources)
			{
				var instance = Instantiate(UIBuilderResourcePrefab);
				instance.transform.SetParent(this.transform);

				var resource = instance.GetComponent<UIResource>();
				if (resource == null)
					resource = instance.AddComponent<UIResource>();

				resource.Toggle.group = GetComponent<UnityEngine.UI.ToggleGroup>();
				resource.SetResource(rsrc);

				_instances[rsrc.Type] = resource;
			}
		}
	}
}
