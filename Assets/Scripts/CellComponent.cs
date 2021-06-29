using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellComponent : MonoBehaviour
{
	public CellData Data
	{
		get { return _data; }
		set
		{
			_data = value;
		}
	}

	public class EventOnClicked
	{
		public CellComponent Cell;
		public int MouseClick;
	}
	public System.Action<EventOnClicked> OnClicked;

	private CellData _data;
	private SpriteRenderer _renderer;

	void Awake()
	{
		_renderer = GetComponent<SpriteRenderer>();
	}

	public void UpdateState(int state)
	{
		_renderer.color = StateToColor(state);
	}

	private Color StateToColor(int state)
	{
		switch (state)
		{
			case -1: return Color.red;
			case 0: return Color.white;
			case 1: return Color.black;
			default: return Color.white;
		}
	}

	public void OnMouseDown()
	{
		Debug.Log("Clicked");
		int c = 0;
		if (Input.GetMouseButtonDown(0))
			c = 1;
		else if (Input.GetMouseButtonDown(1))
			c = 2;
			
		OnClicked?.Invoke(new EventOnClicked{
			Cell = this,
			MouseClick = c
		});
	}
}
