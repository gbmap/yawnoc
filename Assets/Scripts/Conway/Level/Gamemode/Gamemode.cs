using Frictionless;
using Messages.Board;
using UnityEngine;

namespace Conway
{
    public class GamemodeState {}

    public abstract class Gamemode : ScriptableObject
    {
        protected enum EState
        {
            Playing,
            GameOver,
            Win
        }

        public abstract GamemodeState Begin(BoardComponent component);
        protected abstract EState GetState(GamemodeState state);

        public bool OnStep(GamemodeState state, OnStep msg)
        {
            EState stt = GetState(state);
            switch (stt)
            {
                case EState.Win: Win(); break;
                case EState.GameOver: GameOver(); break;
                default: return false;
            }

            return true;
        }

        public virtual void OnCellChanged(GamemodeState state, Messages.Board.OnCellChanged msg) {}
        public virtual void OnResourcesDepleted(GamemodeState state, Messages.Builder.OnBuilderResourcesDepleted msg) {}

        protected virtual void GameOver()
        {
            MessageRouter.RaiseMessage(new Messages.Gameplay.OnGameLost {});
        }

        protected virtual void Win()
        {
            MessageRouter.RaiseMessage(new Messages.Gameplay.OnGameWon {});
        }
    }
}
