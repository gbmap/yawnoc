using Frictionless;
using Messages.Board;
using UnityEngine;

namespace Conway
{
    public abstract class Gamemode : ScriptableObject
    {
        protected enum EState
        {
            Playing,
            GameOver,
            Win
        }

        protected BoardComponent _component;
        protected abstract EState GetState();

        private bool _finished;

        public virtual void Begin(BoardComponent component)
        {
            _component = component;

            MessageRouter.AddHandler<Messages.Board.OnStep>(Cb_OnStep);
        }

        private void Cb_OnStep(OnStep msg)
        {
            if (_finished) 
                return;

            EState state = GetState();
            switch (state)
            {
                case EState.Win: Win(); break;
                case EState.GameOver: GameOver(); break;
                default: return;
            }

            _finished = true;
        }

        protected void GameOver()
        {
            MessageRouter.RaiseMessage(new Messages.Gameplay.OnGameLost {});
        }

        protected void Win()
        {
            MessageRouter.RaiseMessage(new Messages.Gameplay.OnGameWon {});
        }
    }
}
