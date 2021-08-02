using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Frictionless;
using Conway;
using System;
using Messages.Input;
using Messages.Command;

public class BoardShader : MonoBehaviour
{
    Conway.Board board;
    BoardComponent component;
    Texture2D texture;

    Renderer renderer;
    Material material;

    public PainterComponent painter;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        material = renderer.material;
    }

    void Update()
    {
        Vector3 cpos = Camera.main.transform.position;
        cpos.z = 0f;
        transform.position = cpos;
        float ortho = Camera.main.orthographicSize*2f;
        float aspect = Camera.main.aspect;
        transform.localScale = new Vector3(ortho*aspect, ortho, 1f);
    }

    void FixedUpdate()
    {
        Vector3 cpos = Camera.main.transform.position;
        cpos.z = 0f;
        transform.position = cpos;
        float ortho = Camera.main.orthographicSize*2f;
        float aspect = Camera.main.aspect;
        transform.localScale = new Vector3(ortho*aspect, ortho, 1f);

        material.SetVector("_CameraPos", Camera.main.transform.position);
        material.SetVector("_CameraSize", transform.localScale);
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        MessageRouter.AddHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
        MessageRouter.AddHandler<Messages.Input.OnClick>(Cb_OnClick);
        MessageRouter.AddHandler<Messages.Command.SelectCell>(Cb_OnCellSelected);
    }

    void OnDisable()
    {
        MessageRouter.RemoveHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
        MessageRouter.RemoveHandler<Messages.Input.OnClick>(Cb_OnClick);
        MessageRouter.RemoveHandler<Messages.Command.SelectCell>(Cb_OnCellSelected);
    }

    void Cb_OnBoardGenerated(Messages.Board.OnBoardGenerated msg)
    {
        board = msg.Board;
        component = msg.Component;
        texture = new Texture2D(board.Size.x, board.Size.y);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
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
        material.SetVector("_CellSize", csz);
        material.SetVector("_BoardSize", new Vector2(board.Size.x, board.Size.y));
    }

    private void Cb_OnCellChanged(Board.OnCellChangedParams obj)
    {
        texture.SetPixel(obj.Position.x, obj.Position.y, board.Style.GetColor(obj.NewType));
        texture.Apply();
    }

    private void Cb_OnStep(Board b)
    {
        texture.Apply();
        material.SetTexture("_Board", texture);
        material.SetTexture("_PainterTexture", painter.Painter.Texture);
    }

    private void Cb_OnClick(OnClick obj)
    {
        material.SetFloat("_ClickTime", Time.time);
        material.SetVector("_ClickPos", component.WorldToBoardF(obj.WorldPosition)/board.Size);
        Vector3 pos = component.WorldToBoardF(obj.WorldPosition);
    }

    private void Cb_OnCellSelected(SelectCell obj)
    {
        material.SetVector("_SelectedCell", new Vector2(obj.Position.x, obj.Position.y));
    }

}
