using System.Collections.Generic;
using System.Collections.Specialized;

namespace BillAcceptorTest
{
    internal class FixedObservableLinkedList<T> : LinkedList<T>, INotifyCollectionChanged
    {
        private readonly object syncObject = new object();

        private int Size { get; }

        public FixedObservableLinkedList(int size)
        {
            Size = size;
        }


        public void Add(T obj)
        {
            AddFirst(obj);
            lock (syncObject)
            {
                while (Count > Size)
                {
                    RemoveLast();
                }
            }

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset, null));
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        private void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
        }
    }
}