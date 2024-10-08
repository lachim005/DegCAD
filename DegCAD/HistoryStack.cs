﻿using System;
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
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return list.GetEnumerator();
        }
    }
}
