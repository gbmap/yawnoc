using System.Collections.Generic;
using System.Linq;
using Frictionless;
using Messages.Builder;
using UnityEngine;

namespace Conway
{
    [CreateAssetMenu(
        menuName="Conway/Gamemode/Eliminate Cell", 
        fileName="GamemodeEliminateCell"
    )]
    public class GamemodeEliminateCell : Gamemode
    {
        public ECellType[] TargetCells;

        private Dictionary<ECellType, int> CurrentCells;
        private bool _resourcesDepleted;

        public override void Begin(BoardComponent component)
        {
            base.Begin(component);

            CurrentCells = new Dictionary<ECellType, int>();
            System.Array.ForEach(TargetCells, x => CurrentCells[x] = 0);

            component.Board.ForEachCell(delegate(Board.ForEachCellParams p)
            {
                if (!TargetCells.Contains(p.State))
                    return;

                int cellCount = -1;
                if (!CurrentCells.TryGetValue(p.State, out cellCount))
                    return;
                
                CurrentCells[p.State] = cellCount + 1;
            });

            MessageRouter.AddHandler<Messages.Board.OnCellChanged>(
                Cb_OnCellChanged
            );

            MessageRouter.AddHandler<Messages.Builder.OnBuilderResourcesDepleted>(
                Cb_OnResourcesDepleted
            );
        }

        protected override EState GetState()
        {
            if (CurrentCells.All(x=>x.Value == 0))
                return EState.Win;

            else if (_resourcesDepleted)
                return EState.GameOver;

            return EState.Playing;
        }

        private void Cb_OnCellChanged(Messages.Board.OnCellChanged msg)
        {
            ECellType cell = msg.OldType;
            if (!TargetCells.Contains(cell))
                return;

            int cellCount = -1;
            if (!CurrentCells.TryGetValue(cell, out cellCount))
                return;

            CurrentCells[cell] = cellCount-1;
        }

        private void Cb_OnResourcesDepleted(OnBuilderResourcesDepleted obj)
        {
            _resourcesDepleted = true;           
        }
    }
}
