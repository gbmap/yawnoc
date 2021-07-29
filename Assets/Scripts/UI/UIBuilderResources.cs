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
			MessageRouter.AddHandler<Messages.Builder.OnBuilderResourceUpdated>(Cb_OnResourceUpdated);
		}

		void OnDisable()
		{
			MessageRouter.RemoveHandler<Messages.Builder.OnBuilderResourcesCreated>(Cb_OnResourcesCreated);
			MessageRouter.RemoveHandler<Messages.Builder.OnBuilderResourceUpdated>(Cb_OnResourceUpdated);
		}

        void Cb_OnResourcesCreated(Messages.Builder.OnBuilderResourcesCreated msg)
		{
            SpawnUIResources(msg.Resources, Conway.LevelLoader.Instance.Style);
		}

        void Cb_OnResourceUpdated(Messages.Builder.OnBuilderResourceUpdated obj)
        {
			_instances[obj.Resource.Type].SetResource(obj.Resource);
        }

		void ClearUIResources()
		{
			foreach (UIResource instance in _instances.Values)
				Destroy(instance.gameObject);

			_instances.Clear();
		}

		void SpawnUIResources(
			List<Conway.Builder.BuildResource> resources,
			Conway.Config.BoardStyle style
		) {
			ClearUIResources();

			foreach (Conway.Builder.BuildResource rsrc in resources)
			{
				var instance = Instantiate(UIBuilderResourcePrefab);
				instance.transform.SetParent(this.transform);
				instance.transform.localScale = Vector3.one;

				var resource = instance.GetComponent<UIResource>();
				if (resource == null)
					resource = instance.AddComponent<UIResource>();

				resource.Toggle.group = GetComponent<UnityEngine.UI.ToggleGroup>();
				resource.Style = style;
				resource.SetResource(rsrc);

				_instances[rsrc.Type] = resource;
			}
		}
	}
}
