using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frictionless;
using Conway;
using System;
using Messages.Input;

public class BoardShader : MonoBehaviour
{
    Conway.Board board;
    BoardComponent component;
    Texture2D texture;

    Renderer renderer;
    Material material;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        material = renderer.material;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        MessageRouter.AddHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
        MessageRouter.AddHandler<Messages.Input.OnClick>(Cb_OnClick);
    }

    void OnDisable()
    {
        MessageRouter.RemoveHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
        MessageRouter.RemoveHandler<Messages.Input.OnClick>(Cb_OnClick);
    }

    void Cb_OnBoardGenerated(Messages.Board.OnBoardGenerated msg)
    {
        board = msg.Board;
        component = msg.Component;
        texture = new Texture2D(board.Size.x, board.Size.y);
        texture.filterMode = FilterMode.Point;
        board.ForEachCell(delegate(Board.ForEachCellParams p)
        {
            texture.SetPixel(p.Position.x, p.Position.y, board.Style.GetColor(p.State));
        });
        texture.Apply();

        Vector3 csz = msg.Component.CellSize;
        Vector2Int bsz = msg.Board.Size;

        transform.position = msg.Component.transform.position - csz/2f;
        transform.localScale = new Vector3(csz.x*bsz.x, csz.y*bsz.y, 1.0f);

        board.OnCellChanged += Cb_OnCellChanged;
        board.OnStep += Cb_OnStep;

        float x = csz.x / board.Size.x;
        float y = csz.y / board.Size.y;

        material.SetTexture("_Board", texture);
        material.SetVector("_CellSize", new Vector2(x,y));
    }

    private void Cb_OnCellChanged(Board.OnCellChangedParams obj)
    {
        texture.SetPixel(obj.Position.x, obj.Position.y, board.Style.GetColor(obj.NewType));
        //texture.Apply();
    }

    private void Cb_OnStep(Board b)
    {
        texture.Apply();
        material.SetTexture("_Board", texture);
    }

    private void Cb_OnClick(OnClick obj)
    {
        material.SetFloat("_ClickTime", Time.time);
        material.SetVector("_ClickPos", component.WorldToBoardF(obj.WorldPosition)/board.Size);
    }
}
