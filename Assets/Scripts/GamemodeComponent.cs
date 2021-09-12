using Frictionless;
using Messages.Board;
using Messages.Builder;
using UnityEngine;

namespace Conway
{
    public class GamemodeComponent : MonoBehaviour
    {
        public Gamemode Gamemode;
        private GamemodeState _state;

        public bool IsValid
        {
            get { return _state != null && Gamemode != null; }
        }

        void OnEnable()
        {
            // MessageRouter.AddHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
            MessageRouter.AddHandler<Messages.Board.OnStep>(Cb_OnStep);
            MessageRouter.AddHandler<Messages.Board.OnCellChanged>(Cb_OnCellChanged);
            MessageRouter.AddHandler<Messages.Builder.OnBuilderResourcesDepleted>(Cb_OnResourcesDepleted);
        }

        void OnDisable()
        {
            // MessageRouter.RemoveHandler<Messages.Board.OnBoardGenerated>(Cb_OnBoardGenerated);
            MessageRouter.RemoveHandler<Messages.Board.OnStep>(Cb_OnStep);
            MessageRouter.RemoveHandler<Messages.Board.OnCellChanged>(Cb_OnCellChanged);
            MessageRouter.RemoveHandler<Messages.Builder.OnBuilderResourcesDepleted>(Cb_OnResourcesDepleted);
        }

        private void StartGamemode(BoardComponent component)
        {
            _state = Gamemode.Begin(component);
        }

        public void Begin(Gamemode gamemode, BoardComponent component)
        {
            Gamemode = gamemode;
            _state = Gamemode.Begin(component);
        }

        private void Cb_OnStep(OnStep obj)
        {
            if (!IsValid)
                return;

            if (Gamemode.OnStep(_state, obj))
                _state = null;
        }

        private void Cb_OnCellChanged(OnCellChanged msg)
        {
            if (!IsValid)
                return;

            Gamemode.OnCellChanged(_state, msg);
        }

        private void Cb_OnResourcesDepleted(OnBuilderResourcesDepleted msg)
        {
            if (!IsValid)
                return;

            Gamemode.OnResourcesDepleted(_state, msg);
        }
    }
}
