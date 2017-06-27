﻿using System.Collections;
using System.Collections.Generic;

namespace InvisualRest
{
  public class ListResponse<T> : IList<T>
  {
    public List<T> Items { get; set; } = new List<T>();

    public T this[int index] { get => Items[index]; set => Items[index] = value; }
    public int Count => Items.Count;
    public bool IsReadOnly => false;
    public void Add(T item) => Items.Add(item);
    public void Clear() => Items.Clear();
    public bool Contains(T item) => Items.Contains(item);
    public void CopyTo(T[] array, int arrayIndex) => Items.CopyTo(array, arrayIndex);
    public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();
    public int IndexOf(T item) => Items.IndexOf(item);
    public void Insert(int index, T item) => Items.Insert(index, item);
    public bool Remove(T item) => Items.Remove(item);
    public void RemoveAt(int index) => Items.RemoveAt(index);
    IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();
  }
}
