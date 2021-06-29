using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frictionless;

public class CellData
{
	public int State;
	public Vector2Int Position;
}

public enum ECellType
{
	Dead,
	Alive,
	Obstacle = -1
}

public class CollectiblePool
{
	public System.Action OnCollectiblesEnded;

	private GameObject _prefab;
	private Dictionary<Vector2Int, CellComponent> Collectibles;

	private BoardComponent _board;

	public CollectiblePool(BoardComponent board, GameObject prefab)
	{
		_board  = board;
		_prefab = prefab;
	}

	public void GeneratePerlin(Conway.Board b)
	{
		Collectibles = new Dictionary<Vector2Int, CellComponent>();

		b.ForEachCell(delegate(Conway.Board.ForEachCellParams p)
		{
			Vector2 coords;
			coords.x = ((float)p.Position.x / b.Size.x) * 100f;
			coords.y = ((float)p.Position.y / b.Size.y) * 100f;
			float v = Mathf.PerlinNoise(coords.x, coords.y);

			Debug.Log(v);
			if (v > 0.75f)
			{
				Debug.Log("Spawning collectible.");
				var collectible = SpawnCollectible(p.Position.x, p.Position.y);
				Collectibles[p.Position] = collectible.GetComponent<CellComponent>(); 
			}
		});

		b.OnCellChanged += Cb_OnCellChanged; 
	}

	private GameObject SpawnCollectible(int x, int y)
	{
		var instance = GameObject.Instantiate(_prefab);
		instance.transform.position = _board.GetPosition(new Vector2Int(x, y));
		return instance;
	}

	private void Cb_OnCellChanged(Vector2Int p, int v)
	{
		Debug.Log("Cell changed");
		if (v != 1) return;
		if (!Collectibles.ContainsKey(p)) return;
		Debug.Log("Cell changed 2");

		// Collectible obtained
		GameObject.Destroy(Collectibles[p].gameObject);
		Collectibles.Remove(p);

		if (Collectibles.Count > 0)
			return;

		OnCollectiblesEnded?.Invoke();
	}
}

/*
	Used for handling cross-system messages in-game.
	Serves to reduce the amount of code inside BoardComponent.
*/
public class BoardComponentMailbox
{
	BoardComponent _b;
	public BoardComponentMailbox(BoardComponent c)
	{
		_b = c;
		MessageRouter.AddHandler<Messages.UI.OnPlayButtonClick>(Cb_OnPlayButtonClick);
	}

	~BoardComponentMailbox()
	{
		MessageRouter.RemoveHandler<Messages.UI.OnPlayButtonClick>(Cb_OnPlayButtonClick);
	}

	private void Cb_OnPlayButtonClick(Messages.UI.OnPlayButtonClick msg)
	{
		_b.ToggleTimer();
	}
}

public class BoardComponent : MonoBehaviour
{
	public Vector2Int Size = new Vector2Int(16, 16);
	public GameObject CellPrefab;
	public ECellType CellBrush; 
	public float Margin = 0f;

	public GameObject BrainPrefab;
	private CollectiblePool _brainPool;

	private CellComponent[,] Cells;

	public bool isPlaying;
	public float stepWait = 0.5f;

	private float _stepTimer;
	private Vector2 _cellSize;
	private Vector2 startPosition
	{
		get { return new Vector3(-_cellSize.x*Size.x/2, -_cellSize.y*Size.y/2, 0f); }
	}

	Conway.Board Board;
	BoardComponentMailbox _mailbox;

    void Start()
    {
		Board = new Conway.Board(Size);
		_mailbox = new BoardComponentMailbox(this);
    	GenerateBoard(Board);
    }

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
			UpdateBoard();

		if (!isPlaying)
			return;

		if (_stepTimer >= stepWait)
		{
			_stepTimer = 0f;
			UpdateBoard();
		}

		_stepTimer += Time.deltaTime;
	}

	//============================== 
	//		Timer
	//============================== 

	public void SetTimerPlaying(bool v)
	{
		isPlaying = v;
	}

	public void ToggleTimer()
	{
		isPlaying = !isPlaying;
	}
	
	//=============================
	//		Board Management
	//=============================

	public void GenerateBoard(Conway.Board board)
	{
		Cells = new CellComponent[board.Size.x, board.Size.y];
		_cellSize = CellPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size;

		board.ForEachCell(delegate (Conway.Board.ForEachCellParams p)
		{
			int x = p.Position.x;
			int y = p.Position.y;

			Vector3 position = new Vector3(x, y);
			var instance     = Instantiate(CellPrefab);
			var sprite       = instance.GetComponent<SpriteRenderer>().sprite;
			var sprSize      = sprite.bounds.size;

			var startPos     = new Vector3(
				-sprSize.x*Size.x/2,
				-sprSize.y*Size.y/2
			);

			var pos = new Vector3(sprSize.x*x, sprSize.y*y, 0f);
			instance.transform.position = startPos + pos;

			CellComponent cellComponent = instance.GetComponent<CellComponent>(); 
			cellComponent.OnClicked += OnCellClicked;

			CellData cellData     = new CellData();
			cellData.Position     = new Vector2Int(x, y);
			cellData.State        = 0;

			cellComponent.Data = cellData; 
			Cells[x, y] = cellComponent;
		});

		board.OnCellChanged += Cb_OnCellChanged;

		_brainPool = new CollectiblePool(this, BrainPrefab);
		_brainPool.GeneratePerlin(board);
	}

	public void Cb_OnCellChanged(Vector2Int p, int v)
	{
		Cells[p.x, p.y].UpdateState(v);
	}

	public void UpdateBoard()
	{
		Board.StepState();
		Board.ApplyRule(new Conway.RuleConway());
		Board.ApplyRule(new Conway.RuleMaintainCell(-1));
	}


	private void OnCellClicked(CellComponent.EventOnClicked evnt)
	{
		var cell = evnt.Cell;
		int tv = (int)CellBrush;
		int v = Board.GetCellCurrent(cell.Data.Position);
		Board.SetCellCurrent(cell.Data.Position, tv);
	}

	public Vector3 GetPosition(Vector2Int p)
	{
		return startPosition + _cellSize * p; 
	}
}

