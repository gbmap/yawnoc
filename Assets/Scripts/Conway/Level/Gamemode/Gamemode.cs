using System.Collections;
using System.Collections.Generic;
using Frictionless;
using UnityEngine;

namespace Conway.Builder
{
    public abstract class Gamemode : ScriptableObject
    {
        public abstract void OnCellChanged(Conway.Board.OnCellChangedParams param);
        public abstract void OnBoardStep(Conway.Board b);

        protected void GameOver()
        {
            MessageRouter.RaiseMessage(new Messages.Gameplay.OnGameLost {

            });
        }

        protected void Win()
        {
            MessageRouter.RaiseMessage(new Messages.Gameplay.OnGameWon {

            });
        }
    }
}
