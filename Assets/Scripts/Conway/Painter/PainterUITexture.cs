using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Frictionless;
using Messages.Painter;

[RequireComponent(typeof(RawImage))]
public class PainterUITexture : MonoBehaviour
{
	RawImage _texture;

	void Awake()
	{
		_texture = GetComponent<RawImage>();
	}

    void OnEnable()
    {
		MessageRouter.AddHandler<OnPainterCreated>(Cb_OnPainterCreated);
		MessageRouter.AddHandler<OnPainterUpdated>(Cb_OnPainterUpdated);
    }

	void OnDisable()
	{
		MessageRouter.RemoveHandler<OnPainterCreated>(Cb_OnPainterCreated);
		MessageRouter.RemoveHandler<OnPainterUpdated>(Cb_OnPainterUpdated);
	}

	void Cb_OnPainterCreated(OnPainterCreated msg)
	{
		_texture.texture = msg.Painter.Painter.Texture;
	}

	void Cb_OnPainterUpdated(OnPainterUpdated msg)
	{
		_texture.texture = msg.Painter.Painter.Texture;
	}
}
