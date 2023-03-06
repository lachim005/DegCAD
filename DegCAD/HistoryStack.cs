using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public class HistoryStack<T> : IEnumerable<T>
    {
        public int Count => list.Count;

        private readonly List<T> list;

        public T this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public HistoryStack()
        {
            list = new();
        }

        public void Push(T item)
        {
            list.Add(item);
        }

        public T Pop()
        {
            var res = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return res;
        }

        public T Peek()
        {
            return list[list.Count - 1];
        }

        public void Clear()
        {
            list.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new HistoryEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new HistoryEnumerator<T>(this);
        }
    }

    internal class HistoryEnumerator<T> : IEnumerator<T>
    {
        public T Current => Stack[index];

        object IEnumerator.Current
        {
            get
            {
                if(Stack[index] is T sth)
                    return sth;
                return 0;
            }
        }

        public HistoryStack<T> Stack { get; private set; }

        private int index = -1;

        public HistoryEnumerator(HistoryStack<T> stack)
        {
            Stack = stack;
        }

        public void Dispose() { }

        public bool MoveNext()
        {
            if (++index < Stack.Count) return true;
            return false;
        }

        public void Reset()
        {
            index = -1;
        }
    }
}
