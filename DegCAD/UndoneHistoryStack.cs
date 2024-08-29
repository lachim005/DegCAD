using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace DegCAD
{
    public class UndoneHistoryStack<T> : IEnumerable<T>
    {
        public int Count => list.Count;

        private readonly List<T> list;

        public T this[int index]
        {
            get => list[index];
            set => list[index] = value;
        }

        public UndoneHistoryStack()
        {
            list = new();
        }

        public void Push(T item)
        {
            list.Insert(0, item);
        }

        public T Pop()
        {
            var res = list[0];
            list.RemoveAt(0);
            return res;
        }

        public T Peek()
        {
            return list[0];
        }

        public void Clear()
        {
            list.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
