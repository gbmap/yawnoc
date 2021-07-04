using System;
using System.Collections.Generic;

using UnityEngine;
using Frictionless;

class BuilderComponent : MonoBehaviour
{
	public Conway.ECellType Brush;
	public Level.BuildResources Resources;

	public void SetResources(Level.BuildResources resources)
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
	}

	void OnDisable()
	{
		MessageRouter.RemoveHandler<Messages.Command.SelectResource>(Cb_OnResourceSelected);
		//MessageRouter.RemoveHandler<Messages.UI.OnResourceSelected>(Cb_OnResourceSelected);
	}

	void Cb_OnResourceSelected(Messages.Command.SelectResource msg)
	{
		Brush = msg.Resource;
	}
}
