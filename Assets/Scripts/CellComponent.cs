using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ECellType = Conway.ECellType;

public class CellComponent : MonoBehaviour
{
	public CellData Data
	{
		get { return _data; }
		set
		{
			_data = value;
			UpdateState(_data.State);
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

	public void UpdateState(ECellType state)
	{
		_data.State = state;
		_renderer.color = StateToColor(state);
	}

	private Color StateToColor(ECellType state)
	{
		return Gameplay.LevelLoader.CellToColor(state);
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
