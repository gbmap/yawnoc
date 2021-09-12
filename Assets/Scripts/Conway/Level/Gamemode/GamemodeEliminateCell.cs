using System.Collections.Generic;
using System.Linq;
using Frictionless;
using Messages.Builder;
using UnityEngine;

namespace Conway
{
    public class CircularBuffer<T>
    {
        T[] _buffer;
        int _idx;
        int _count;

        public int Count { get { return _count; } }
        public int Size { get { return _buffer.Length; } }

        public CircularBuffer(int size)
        {
            _buffer = new T[size];
            _idx = 0;
        }

        public T Get(int index)
        {
            int i = (_idx + 1 + index) % Size;
            return _buffer[i];
        }

        public T Peek()
        {
            return _buffer[_idx];
        }

        public void Push(T item)
        {
            _buffer[_idx] = item;
            _idx = (_idx+1)%Size;
            _count = Mathf.Max(_count, _idx);
        }
    }

    public class GamemodeEliminateCellState : GamemodeState
    {
        public Dictionary<ECellType, int> CurrentCells = new Dictionary<ECellType, int>();
        public CircularBuffer<int> LastAliveCells;
        public bool ResourcesDepleted;
    }
    
    public class GamemodeEliminateCell : Gamemode
    {
        public ECellType[] TargetCells;

        public override GamemodeState Begin(BoardComponent component)
        {
            GamemodeEliminateCellState state = new GamemodeEliminateCellState();
            state.CurrentCells = new Dictionary<ECellType, int>();
            state.CurrentCells[ECellType.Alive] = 0;
            System.Array.ForEach(TargetCells, x => state.CurrentCells[x] = 0);

            state.LastAliveCells = new CircularBuffer<int>(100);
            for (int i = 0; i < state.LastAliveCells.Count; i++)
                state.LastAliveCells.Push(i%2);

            component.Board.ForEachCell(delegate(Board.ForEachCellParams p)
            {
                if (!TargetCells.Contains(p.State) && p.State != ECellType.Alive)
                    return;

                int cellCount = -1;
                if (!state.CurrentCells.TryGetValue(p.State, out cellCount))
                    return;
                
                state.CurrentCells[p.State] = cellCount + 1;
            });

            return state;
        }

        protected override EState GetState(GamemodeState currentState)
        {
            GamemodeEliminateCellState state = currentState as GamemodeEliminateCellState;

            bool noMoreTargetCells = state.CurrentCells.Where(
                x=> TargetCells.Contains(x.Key)
            ).All(x=>x.Value == 0);

            if (noMoreTargetCells)
                return EState.Win;

            state.LastAliveCells.Push(state.CurrentCells[ECellType.Alive]);
            int lastCount = state.LastAliveCells.Get(0);
            bool isConstant = true;
            for (int i = 0; i < state.LastAliveCells.Size; i++)
            {
                int v = state.LastAliveCells.Get(i);
                if (lastCount != v)
                {
                    isConstant = false;
                    break;
                }

                lastCount = state.LastAliveCells.Get(i);
            }
            
            bool noLiveCells = state.CurrentCells[ECellType.Alive] == 0;
            bool resourcesDepleted = state.ResourcesDepleted;

            if (resourcesDepleted && (noLiveCells || isConstant))
            {
                Debug.Log($"[{this.GetInstanceID()}] Defeat. \n Resources Depleted: {resourcesDepleted} \n No Live Cells: {noLiveCells} \n Is Constant: {isConstant} ");
                return EState.GameOver;
            }

            return EState.Playing;
        }

        public override void OnCellChanged(
            GamemodeState currentState, 
            Messages.Board.OnCellChanged msg
        ) {
            base.OnCellChanged(currentState, msg);

            GamemodeEliminateCellState state = 
                currentState as GamemodeEliminateCellState;

            ECellType cellOld = msg.OldType;
            ECellType cellNew = msg.NewType;
            bool watchedOld = state.CurrentCells.ContainsKey(cellOld);
            bool watchedNew = state.CurrentCells.ContainsKey(cellNew);

            if (!watchedOld && !watchedNew)
                return;

            if (cellOld == ECellType.Collectible &&
                cellNew != ECellType.Collectible)
                state.ResourcesDepleted = false;

            if (watchedOld)
                state.CurrentCells[cellOld] = state.CurrentCells[cellOld]-1;

            if (watchedNew)
                state.CurrentCells[cellNew] = state.CurrentCells[cellNew]+1;

            state.LastAliveCells.Push(state.CurrentCells[ECellType.Alive]);
        }

        public override void OnResourcesDepleted(GamemodeState currentState, OnBuilderResourcesDepleted obj)
        {
            base.OnResourcesDepleted(currentState, obj);
            GamemodeEliminateCellState state = currentState as GamemodeEliminateCellState;
            state.ResourcesDepleted = true;
        }
    }
}
