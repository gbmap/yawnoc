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
    
    public class GamemodeEliminateCell : Gamemode
    {
        public ECellType[] TargetCells;

        private Dictionary<ECellType, int> CurrentCells;
        private CircularBuffer<int> LastAliveCells;
        private bool _resourcesDepleted;

        public override void Begin(BoardComponent component)
        {
            base.Begin(component);

            CurrentCells = new Dictionary<ECellType, int>();
            CurrentCells[ECellType.Alive] = 0;
            System.Array.ForEach(TargetCells, x => CurrentCells[x] = 0);

            LastAliveCells = new CircularBuffer<int>(10);
            for (int i = 0; i < LastAliveCells.Count; i++)
                LastAliveCells.Push(i%2);

            component.Board.ForEachCell(delegate(Board.ForEachCellParams p)
            {
                if (!TargetCells.Contains(p.State) && p.State != ECellType.Alive)
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
            bool noMoreTargetCells = CurrentCells.Where(
                x=> TargetCells.Contains(x.Key)
            ).All(x=>x.Value == 0);

            if (noMoreTargetCells)
                return EState.Win;

            LastAliveCells.Push(CurrentCells[ECellType.Alive]);
            int lastCount = LastAliveCells.Get(0);
            bool isConstant = true;
            for (int i = 0; i < LastAliveCells.Size; i++)
            {
                int v = LastAliveCells.Get(i);
                if (lastCount != v)
                {
                    isConstant = false;
                    break;
                }

                lastCount = LastAliveCells.Get(i);
            }
            
            bool noLiveCells = CurrentCells[ECellType.Alive] == 0;

            if (_resourcesDepleted && (noLiveCells || isConstant))
                return EState.GameOver;

            return EState.Playing;
        }

        private void Cb_OnCellChanged(Messages.Board.OnCellChanged msg)
        {
            ECellType cell = msg.OldType;

            int cellCount = -1;
            if (!CurrentCells.TryGetValue(cell, out cellCount))
                return;

            CurrentCells[cell] = cellCount-1;
            LastAliveCells.Push(CurrentCells[ECellType.Alive]);
        }

        private void Cb_OnResourcesDepleted(OnBuilderResourcesDepleted obj)
        {
            _resourcesDepleted = true; 
        }
    }
}
