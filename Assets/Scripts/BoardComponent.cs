using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frictionless;
using ECellType = Conway.ECellType;

public class CellData
{
	public ECellType State;
	public Vector2Int Position;
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
		Collectibles = new Dictionary<Vector2Int, CellComponent>();
	}

	public void Generate(Conway.Board b)
	{
		b.ForEachCell(delegate(Conway.Board.ForEachCellParams p)
		{
			if (p.State != ECellType.Collectible)
				return;

			var collectible = SpawnCollectible(p.Position.x, p.Position.y);
			Collectibles[p.Position] = collectible.GetComponent<CellComponent>(); 
		});

		b.OnCellChanged += Cb_OnCellChanged; 
	}

	public void Destroy()
	{
		foreach (var kvp in Collectibles)
			GameObject.Destroy(kvp.Value.gameObject);

		OnCollectiblesEnded = null;
	}

	public void GeneratePerlin(Conway.Board b)
	{
		b.ForEachCell(delegate(Conway.Board.ForEachCellParams p)
		{
			Vector2 coords;
			coords.x = ((float)p.Position.x / b.Size.x) * 100f;
			coords.y = ((float)p.Position.y / b.Size.y) * 100f;
			float v = Mathf.PerlinNoise(coords.x, coords.y);

			Debug.Log(v);
			if (v > 0.85f)
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
		Debug.Log($"Spawning collectible at {x}, {y}.");
		var instance = GameObject.Instantiate(_prefab);
		instance.transform.position = _board.GetPosition(new Vector2Int(x, y), -0.01f);
		return instance;
	}

	private void Cb_OnCellChanged(Vector2Int p, ECellType v)
	{
		if (v != ECellType.Alive) return;
		if (!Collectibles.ContainsKey(p)) return;

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
		MessageRouter.AddHandler<Messages.UI.OnStepButtonClick>(Cb_OnStepButtonClick);
	}

	~BoardComponentMailbox()
	{
		MessageRouter.RemoveHandler<Messages.UI.OnPlayButtonClick>(Cb_OnPlayButtonClick);
		MessageRouter.RemoveHandler<Messages.UI.OnStepButtonClick>(Cb_OnStepButtonClick);
	}

	private void Cb_OnPlayButtonClick(Messages.UI.OnPlayButtonClick msg)
	{
		_b.ToggleTimer();
	}

	private void Cb_OnStepButtonClick(Messages.UI.OnStepButtonClick msg)
	{
		_b.UpdateBoard();
	}
}

public class BoardComponent : MonoBehaviour
{
	public Vector2Int Size = new Vector2Int(16, 16);
	public GameObject CellPrefab;
	public ECellType CellBrush; 
	public float Margin = 0f;

	public Conway.Rules.Ruleset Ruleset;

	public GameObject BrainPrefab;

	public Data.Level Level;
	private CollectiblePool _brainPool;

	private CellComponent[,] Cells;

	public bool isPlaying;
	public float stepWait = 0.5f;

	private float _stepTimer;
	private Vector2 _cellSize;
	private Vector3 startPosition
	{
		get { return new Vector3(-_cellSize.x*Size.x/2, -_cellSize.y*Size.y/2, 0f); }
	}

	Conway.Board Board;
	BoardComponentMailbox _mailbox;

	public class OnBoardGeneratedParams
	{
		public BoardComponent Component;
		public Conway.Board Board;
	}
	public System.Action<OnBoardGeneratedParams> OnBoardGenerated;

    void Start()
    {
		_mailbox = new BoardComponentMailbox(this);
		Conway.Board b = null;
		if (Level == null)
			b = new Conway.Board(Size, Ruleset);
		else
			b = Gameplay.LevelLoader.Load(Level, Ruleset);
		GenerateBoard(b);
    }

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
			UpdateBoard();

		if (Input.GetKeyDown(KeyCode.L) && Level != null)
			GenerateBoard(Gameplay.LevelLoader.Load(Level, Ruleset));

		if (Input.GetKeyDown(KeyCode.Space))
			ToggleTimer();

		if (Input.GetKeyDown(KeyCode.C))
			GenerateBoard(new Conway.Board(Size, Ruleset));

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
		if (Board != null)
			DestroyBoard();

		Board = board;

		_cellSize = CellPrefab.GetComponent<SpriteRenderer>().sprite.bounds.size;

		if (CellPrefab != null)
		{
			InstantiateCells(board.Size);
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

				//var pos = new Vector3(sprSize.x*x, sprSize.y*y, 0f);
				instance.transform.position = GetPosition(p.Position);

				CellComponent cellComponent = instance.GetComponent<CellComponent>(); 
				cellComponent.OnClicked += OnCellClicked;

				CellData cellData     = new CellData();
				cellData.Position     = p.Position;
				cellData.State        = p.State;

				cellComponent.Data = cellData; 
				Cells[x, y] = cellComponent;
			});
		}

		board.OnCellChanged += Cb_OnCellChanged;

		if (BrainPrefab != null)
		{
			_brainPool = new CollectiblePool(this, BrainPrefab);
			_brainPool.Generate(board);
			_brainPool.OnCollectiblesEnded += Cb_OnCollectiblesEnded;
		}

		OnBoardGenerated?.Invoke(new OnBoardGeneratedParams
		{
			Component = this,
			Board = board
		});

		MessageRouter.RaiseMessage(new Messages.Board.OnBoardGenerated
		{
			Component = this,
			Board = board
		});
	}

	private void DestroyBoard()
	{
		if (Board != null)
		{
			DestroyCells();
			Board = null;
		}

		if (_brainPool != null)
		{
			_brainPool.Destroy();
			_brainPool = null;
		}
	}

	private void InstantiateCells(Vector2Int size)
	{
		Cells = new CellComponent[size.x, size.y];
	}

	private void DestroyCells()
	{
		if (Cells == null) return;
		foreach (CellComponent cell in Cells)
			GameObject.Destroy(cell.gameObject);
		Cells = null;
	}

	public void Cb_OnCellChanged(Vector2Int p, ECellType v)
	{
		if (Cells == null) return;
		Cells[p.x, p.y].UpdateState(v);
	}

	public void Cb_OnCollectiblesEnded()
	{
		MessageRouter.RaiseMessage(new Messages.Gameplay.OnGameWon());
	}

	public void UpdateBoard()
	{
		Board.StepState();
	}

	private void OnCellClicked(CellComponent.EventOnClicked evnt)
	{
		var cell = evnt.Cell;
		ECellType v = Board.GetCellCurrent(cell.Data.Position);
		Board.SetCellCurrent(cell.Data.Position, CellBrush);
	}

	public Vector3 GetPosition(Vector2Int p, float zOffset=0f)
	{
		Vector2 pc = _cellSize * p;
		return startPosition + new Vector3(pc.x, pc.y, 0f) + Vector3.forward * zOffset; 
	}
}

