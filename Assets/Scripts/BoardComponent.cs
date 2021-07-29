using System.Collections.Generic;
using UnityEngine;
using Frictionless;
using ECellType = Conway.ECellType;
using Conway;
using Messages.Input;
using System;

public class CellData
{
	public ECellType State;
	public Vector2Int Position;
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

		MessageRouter.AddHandler<Messages.Command.PutCell>(Cb_PutCell); 
		MessageRouter.AddHandler<Messages.Command.PutCellWorld>(Cb_PutCellWorld);

		MessageRouter.AddHandler<Messages.Input.OnClick>(Cb_OnClick);
	}

	~BoardComponentMailbox()
	{
		MessageRouter.RemoveHandler<Messages.UI.OnPlayButtonClick>(Cb_OnPlayButtonClick);
		MessageRouter.RemoveHandler<Messages.UI.OnStepButtonClick>(Cb_OnStepButtonClick);

		MessageRouter.RemoveHandler<Messages.Command.PutCell>(Cb_PutCell);
		MessageRouter.RemoveHandler<Messages.Command.PutCellWorld>(Cb_PutCellWorld);

		MessageRouter.RemoveHandler<Messages.Input.OnClick>(Cb_OnClick);
	}

    private void Cb_OnClick(OnClick obj)
    {
		Vector2Int pos = _b.WorldToBoard(obj.WorldPosition); 
		if (!_b.IsInsideBoard(pos))
		{
			Debug.Log("Not inside board.");
			return; 
		}

		MessageRouter.RaiseMessage(new Messages.Command.SelectCell {
			Position = pos,
			Type = _b.Board.GetCellCurrent(pos)
		});
    }

    private void Cb_OnPlayButtonClick(Messages.UI.OnPlayButtonClick msg)
	{
		_b.ToggleTimer();
	}

	private void Cb_OnStepButtonClick(Messages.UI.OnStepButtonClick msg)
	{
		_b.UpdateBoard();
	}

	private void Cb_PutCell(Messages.Command.PutCell msg)
	{
		Debug.Log(msg.Position);
		if (!_b.Board.SetCell(msg.Position, msg.Type))
			return;

		MessageRouter.RaiseMessage(new Messages.Gameplay.OnCellPlaced {
			Cell = msg.Type
		});
	}

	private void Cb_PutCellWorld(Messages.Command.PutCellWorld msg)
	{
		Vector2Int boardPosition = _b.WorldToBoard(msg.WorldPosition);
		Debug.Log(msg.WorldPosition);

		MessageRouter.RaiseMessage(new Messages.Command.PutCell {
			Position = boardPosition,
			Type = msg.Type
		});
	}
}

public class BoardComponent : MonoBehaviour
{
	[Header("Definition")]
	public GameObject CellPrefab;
	public GameObject BrainPrefab;
	public float Margin = 0f;

	//[Header("Configuration")]
	//public Conway.Rules.Ruleset Ruleset;

	[Header("Time")]
	public bool isPlaying;
	public float stepWait = 0.5f;

	private CellComponent[,] Cells;

	private float _stepTimer;
	private Vector2 _cellSize;
	public Vector2 CellSize { get { return _cellSize; } }
	private Vector3 startPosition
	{
		get { return new Vector3(-_cellSize.x*Board.Size.x/2, -_cellSize.y*Board.Size.y/2, 0f); }
	}

	public Conway.Board Board { get; private set; }
	BoardComponentMailbox _mailbox;

    void Start()
    {
		_mailbox = new BoardComponentMailbox(this);
    }

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
			UpdateBoard();

		if (Input.GetKeyDown(KeyCode.Space))
			ToggleTimer();

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
					-sprSize.x*Mathf.Floor(board.Size.x/2),
					-sprSize.y*Mathf.Floor(board.Size.y/2)
				);

				//var pos = new Vector3(sprSize.x*x, sprSize.y*y, 0f);
				instance.transform.SetParent(transform);
				instance.transform.localPosition = GetPosition(p.Position);

				CellData cellData     = new CellData();
				cellData.Position     = p.Position;
				cellData.State        = p.State;

				CellComponent cellComponent = instance.GetComponent<CellComponent>(); 
				cellComponent.Style = board.Style; 
				cellComponent.Data  = cellData; 
				//cellComponent.OnClicked += OnCellClicked;
				Cells[x, y] = cellComponent;
			});
		}

		board.OnCellChanged += Cb_OnCellChanged;

		MessageRouter.RaiseMessage(new Messages.Board.OnBoardGenerated
		{
			Component      = this,
			Board 		   = board
		});
	}

	private void DestroyBoard()
	{
		if (Board != null)
		{
			DestroyCells();
			Board = null;
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

	public void Cb_OnCellChanged(Conway.Board.OnCellChangedParams param)
	{
		if (Cells == null) return;
		var p = param.Position;
		Cells[p.x, p.y].UpdateState(param.NewType);
	}

	public void Cb_OnCollectiblesEnded()
	{
		MessageRouter.RaiseMessage(new Messages.Gameplay.OnGameWon());
	}

	public void UpdateBoard()
	{
		Board.StepState();
	}

	public Vector2Int WorldToBoard(Vector3 position)
	{
		var bp  = WorldToBoardF(position);
		return new Vector2Int(Mathf.FloorToInt(bp.x), Mathf.FloorToInt(bp.y));
	}

	public Vector2 WorldToBoardF(Vector3 position)
	{
		var csz = new Vector3(_cellSize.x, _cellSize.y)/ 2f;
		Vector3 p = (position) / _cellSize; 
		Vector3 bsz = new Vector3(Board.Size.x, Board.Size.y, 0f);
		return (bsz/2) + p;
	}

	public Vector3 GetPosition(Vector2Int p, float zOffset=0f)
	{
		Vector2 pc = _cellSize * p;
		var forwardOffset = Vector3.forward * zOffset;
		var pos = new Vector3(pc.x, pc.y, 0f);
		return startPosition + pos + forwardOffset; 
	}

	public bool IsInsideBoard(Vector2Int p)
	{
		return p.x >= 0 && p.x < Board.Size.x &&
			   p.y >= 0 && p.y < Board.Size.y;	
	}
}

