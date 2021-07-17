using System;
using System.Collections.Generic;

using UnityEngine;
using Frictionless;
using Messages.Gameplay;

class BuilderComponent : MonoBehaviour
{
	public Conway.ECellType Brush;
	public Conway.Builder.BuildResources Resources;

	public void SetResources(Conway.Builder.BuildResources resources)
	{
		Resources = resources;

		MessageRouter.RaiseMessage(new Messages.Builder.OnBuilderResourcesCreated
		{
			Resources = Resources
		});
	}

	void OnEnable()
	{
		//MessageRouter.AddHandler<Messages.UI.OnResourceSelected>(Cb_OnResourceSelected);
		MessageRouter.AddHandler<Messages.Command.SelectResource>(Cb_OnResourceSelected);
		MessageRouter.AddHandler<Messages.Input.OnClick>(Cb_OnClick);
		MessageRouter.AddHandler<Messages.Gameplay.OnCellPlaced>(Cb_OnCellPlaced);
		MessageRouter.AddHandler<Messages.Gameplay.OnCollectibleObtained>(Cb_OnCollectibleObtained);
	}

	void OnDisable()
	{
		MessageRouter.RemoveHandler<Messages.Command.SelectResource>(Cb_OnResourceSelected);
		MessageRouter.RemoveHandler<Messages.Input.OnClick>(Cb_OnClick);
		MessageRouter.RemoveHandler<Messages.Gameplay.OnCellPlaced>(Cb_OnCellPlaced);
		MessageRouter.RemoveHandler<Messages.Gameplay.OnCollectibleObtained>(Cb_OnCollectibleObtained);
		//MessageRouter.RemoveHandler<Messages.UI.OnResourceSelected>(Cb_OnResourceSelected);
	}

    void Cb_OnResourceSelected(Messages.Command.SelectResource msg)
	{
		Brush = msg.Resource;
	}

	void Cb_OnClick(Messages.Input.OnClick msg)
	{
		var resource = Resources.Get(Brush);
		if (resource.Count <= 0) return;

		MessageRouter.RaiseMessage(new Messages.Command.PutCellWorld{
			WorldPosition = msg.WorldPosition,
			Type = Brush
		});
	}

    void Cb_OnCellPlaced(OnCellPlaced obj)
    {
		Conway.Builder.BuildResource resource = Resources.Get(obj.Cell);
		resource.Count--;

		MessageRouter.RaiseMessage(new Messages.Builder.OnBuilderResourceUpdated {
			Resource = resource
		});
    }

    private void Cb_OnCollectibleObtained(OnCollectibleObtained obj)
    {
		Conway.Builder.BuildResource resource = Resources.Get(obj.Cell);
		if (resource == null)
			return;

		resource.Count++;

		MessageRouter.RaiseMessage(new Messages.Builder.OnBuilderResourceUpdated {
			Resource = resource
		});
    }

}
