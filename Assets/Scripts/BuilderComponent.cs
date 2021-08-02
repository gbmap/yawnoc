using System;
using System.Collections.Generic;

using UnityEngine;
using Frictionless;
using Messages.Gameplay;
using Messages.Command;
using Messages.Input;

public abstract class BuilderBehaviour
{
	private BuilderComponent _component;

	public BuilderBehaviour(BuilderComponent component)
	{
		_component = component;
	}

    public virtual void Cb_OnResourceSelected(Messages.Command.SelectResource msg)
	{
		_component.Brush = msg.Resource;
	}

	public virtual void Cb_OnClick(Messages.Input.OnClick msg) {}
	public virtual void Cb_OnCellSelected(Messages.Command.SelectCell msg){}

	protected void PlaceCell(Vector2Int position)
	{
		var resource = _component.Get(_component.Brush);
		if (resource.Count <= 0) return;

		MessageRouter.RaiseMessage(new Messages.Command.PutCell{
			Position = position,
			Type = _component.Brush
		});
	}

	protected void PlaceCell(Vector3 worldPosition)
	{
		var resource = _component.Get(_component.Brush);
		if (resource.Count <= 0) return;

		MessageRouter.RaiseMessage(new Messages.Command.PutCellWorld {
			WorldPosition = worldPosition,
			Type = _component.Brush
		});
	}
}

public class MouseBuilderBehaviour : BuilderBehaviour
{
	public MouseBuilderBehaviour(BuilderComponent component)
		: base(component)
	{ }

    public override void Cb_OnClick(OnClick msg)
    {
		PlaceCell(msg.WorldPosition);
    }
}

public class TouchBuilderBehaviour : BuilderBehaviour
{
	public Vector2Int SelectedCell = new Vector2Int(int.MinValue, int.MinValue);

	public TouchBuilderBehaviour(BuilderComponent component)
		: base(component)
	{ }

    public override void Cb_OnCellSelected(SelectCell msg)
    {
        base.Cb_OnCellSelected(msg);
		SelectedCell = msg.Position;
    }

    public override void Cb_OnResourceSelected(SelectResource msg)
    {
		if (SelectedCell == new Vector2Int(int.MinValue, int.MinValue))
			return; 

        base.Cb_OnResourceSelected(msg);
		PlaceCell(SelectedCell);
    }
}

public class BuilderComponent : MonoBehaviour
{
	public Conway.ECellType Brush;
	public Conway.Builder.BuildResources Resources;

	public enum EBehaviour
	{
		Touch,
		Mouse
	}
	public EBehaviour behaviour;
	BuilderBehaviour builderBehaviour;

	void Awake()
	{
		if (behaviour == EBehaviour.Mouse)
			builderBehaviour = new MouseBuilderBehaviour(this);
		else
			builderBehaviour = new TouchBuilderBehaviour(this);
	}

	public Conway.Builder.BuildResource Get(Conway.ECellType cellType) 
	{
		return Resources.Get(cellType);
	}

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
		if (builderBehaviour != null)
		{
			MessageRouter.AddHandler<Messages.Command.SelectResource>(builderBehaviour.Cb_OnResourceSelected);
			MessageRouter.AddHandler<Messages.Input.OnClick>(builderBehaviour.Cb_OnClick);
			MessageRouter.AddHandler<Messages.Command.SelectCell>(builderBehaviour.Cb_OnCellSelected);
		}

		MessageRouter.AddHandler<Messages.Gameplay.OnCellPlaced>(Cb_OnCellPlaced);
		MessageRouter.AddHandler<Messages.Gameplay.OnCollectibleObtained>(Cb_OnCollectibleObtained);
	}

	void OnDisable()
	{
		if (builderBehaviour != null)
		{
			MessageRouter.RemoveHandler<Messages.Command.SelectResource>(builderBehaviour.Cb_OnResourceSelected);
			MessageRouter.RemoveHandler<Messages.Input.OnClick>(builderBehaviour.Cb_OnClick);
			MessageRouter.RemoveHandler<Messages.Command.SelectCell>(builderBehaviour.Cb_OnCellSelected);
		}

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
