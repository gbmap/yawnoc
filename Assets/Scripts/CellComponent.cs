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
		return Gameplay.BoardLoader.CellToColor(state);
	}
}
