using Frictionless;
using Messages.Board;
using UnityEngine;

namespace Conway
{
    public class GamemodeComponent : MonoBehaviour
    {
        public Gamemode Gamemode;

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
            // soft copy
            Gamemode = Instantiate(Gamemode);
            Gamemode.Begin(obj.Component);
        }
    }
}
