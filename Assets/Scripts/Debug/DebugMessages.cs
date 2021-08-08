using Frictionless;
using Messages.Board;
using UnityEngine;

public class DebugMessages : MonoBehaviour
{
    void OnEnable()
    {
        MessageRouter.AddHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
    }

    void OnDisable()
    {
        MessageRouter.RemoveHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
    }

    private void Cb_OnBoardGenerated(OnBoardGenerated obj)
    {
        Debug.Log("Board generated.");
    }
}
