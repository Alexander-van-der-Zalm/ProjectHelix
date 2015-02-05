using System.Collections;

namespace Framework.Collections
{
    public interface IQueue
    {
        int Count { get; }

        object Get();

        void Add(object obj);
    }

    public interface IQueue<T>
    {
        void Add(T obj);

        //T Get();
    }
}