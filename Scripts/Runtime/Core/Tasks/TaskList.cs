using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Z3.NodeGraph.Core;
using Z3.Utils.ExtensionMethods;

namespace Z3.NodeGraph.Tasks
{
    [Serializable]
    public class TaskList<T> : ISubAssetList<T> where T : Task
    {
        [SerializeField] protected List<T> taskList = new();

        // IList implementation
        bool IList.IsFixedSize => ((IList)taskList).IsFixedSize;
        bool ICollection.IsSynchronized => ((ICollection)taskList).IsSynchronized;
        object ICollection.SyncRoot => ((ICollection)taskList).SyncRoot;
        bool IList.IsReadOnly => ((IList)taskList).IsReadOnly;
        public int Count => taskList.Count; // IColleciton
        object IList.this[int index]
        {
            get => ((IList)taskList)[index];
            set => ((IList)taskList)[index] = value;
        }

        // IList<T> implementation
        int ICollection<T>.Count => taskList.Count;
        bool ICollection<T>.IsReadOnly => ((ICollection<T>)taskList).IsReadOnly;
        T IList<T>.this[int index]
        {
            get => taskList[index];
            set => taskList[index] = value;
        }

        // TaskList implementation
        public virtual void StartTaskList() { }

        public virtual void StopTaskList() { }

        public string GetInfo()
        {
            if (taskList.Count == 0)
                return "Empty".AddRichTextColor(Color.gray);

            string info = string.Empty;
            foreach (T task in taskList)
            {
                if (task)
                {
                    info += "\n" + task.ToString();
                }
                else
                {
                    info += "\n" + "Missing".AddRichTextColor(Color.red);
                }
            }

            return info.TrimStart('\n');
        }

        public override string ToString() => GetInfo();

        // IList implementation
        int IList.Add(object value) => ((IList)taskList).Add(value);
        void IList.Clear() => taskList.Clear();
        bool IList.Contains(object value) => taskList.Contains((T)value);
        int IList.IndexOf(object value) => taskList.IndexOf((T)value);
        void IList.Insert(int index, object value) => taskList.Insert(index, (T)value);
        void IList.Remove(object value) => taskList.Remove((T)value);
        void IList.RemoveAt(int index) => taskList.RemoveAt(index);
        void ICollection.CopyTo(Array array, int index) => ((ICollection)taskList).CopyTo(array, index);
        IEnumerator IEnumerable.GetEnumerator() => taskList.GetEnumerator();

        // IList<T> implementation
        int IList<T>.IndexOf(T item) => taskList.IndexOf(item);
        void IList<T>.Insert(int index, T item) => taskList.Insert(index, item);
        void IList<T>.RemoveAt(int index) => taskList.RemoveAt(index);
        void ICollection<T>.Add(T item) => taskList.Add(item);
        void ICollection<T>.Clear() => taskList.Clear();
        bool ICollection<T>.Contains(T item) => taskList.Contains(item);
        void ICollection<T>.CopyTo(T[] array, int arrayIndex) => taskList.CopyTo(array, arrayIndex);
        bool ICollection<T>.Remove(T item) => taskList.Remove(item);
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => taskList.GetEnumerator();


        // Implicit conversion operator
        public static implicit operator List<T>(TaskList<T> taskList) => taskList.taskList;
    } 
}
