using System.Collections;
using System.Collections.Generic;

namespace Framework.Collections
{
    public class Stack<T> : IStack, IStack<T>
    {
        private List<T> list;

        public Stack()
        {
            list = new List<T>();
        }

        public int Count
        {
            get { return list.Count; }
        }

        public object Get()
        {
            if (list.Count == 0)
                return null;
            int i = list.Count - 1;
            object obj = list[i];
            list.RemoveAt(i);
            return obj;
        }

        public void Add(object obj)
        {
            list.Add((T)obj);
        }

        public void Add(T obj)
        {
            list.Add(obj);
        }
    }
}

