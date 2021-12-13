﻿using System;
using System.Collections;
using System.Collections.Generic;

namespace FenomPlus.SDK.Core.Utils
{
    [Serializable]
    public class SynchronizedList<T> : IList<T>
    {
        private List<T> _list;
        private object _root;

        public SynchronizedList()
        {
            _list = new List<T>();
            _root = ((ICollection)_list).SyncRoot;
        }

        public SynchronizedList(List<T> list)
        {
            _list = list;
            _root = ((ICollection)list).SyncRoot;
        }

        public int Count
        {
            get
            {
                lock (_root)
                {
                    return _list.Count;
                }
            }
        }

        public bool IsReadOnly => ((ICollection<T>)_list).IsReadOnly;

        public void Add(T item)
        {
            lock (_root)
            {
                _list.Add(item);
            }
        }

        public void Clear()
        {
            lock (_root)
            {
                _list.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock (_root)
            {
                return _list.Contains(item);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            lock (_root)
            {
                _list.CopyTo(array, arrayIndex);
            }
        }

        public bool Remove(T item)
        {
            lock (_root)
            {
                return _list.Remove(item);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (_root)
            {
                return _list.GetEnumerator();
            }
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            lock (_root)
            {
                return ((IEnumerable<T>)_list).GetEnumerator();
            }
        }

        public T this[int index]
        {
            get
            {
                lock (_root)
                {
                    return _list[index];
                }
            }
            set
            {
                lock (_root)
                {
                    _list[index] = value;
                }
            }
        }

        public int IndexOf(T item)
        {
            lock (_root)
            {
                return _list.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock (_root)
            {
                _list.Insert(index, item);
            }
        }

        public void RemoveAt(int index)
        {
            lock (_root)
            {
                _list.RemoveAt(index);
            }
        }
    }
}