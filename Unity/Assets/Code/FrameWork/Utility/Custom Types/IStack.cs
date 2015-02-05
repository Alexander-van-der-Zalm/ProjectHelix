using System.Collections;

namespace Framework.Collections
{
    public interface IStack
    {
        int Count { get; }
        object Get();
        void Add(object obj);
    }

    public interface IStack<T>
    {
        void Add(T obj);
    }
}
