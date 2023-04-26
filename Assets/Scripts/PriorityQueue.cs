using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PriorityQueue<T>
    where T : class, IComparable<T>
{
    private List<T> heap = new List<T>();
    private readonly Comparison<T> comparison;

    public PriorityQueue(Comparison<T> comparison)
    {
        this.comparison = comparison;
    }

    public int Count => heap.Count;

    public void Enqueue(T item)
    {
        heap.Add(item);
        int ci = heap.Count - 1;
        while (ci > 0)
        {
            int pi = (ci - 1) / 2;
            if (comparison(heap[ci], heap[pi]) >= 0)
                break;
            T tmp = heap[ci]; heap[ci] = heap[pi]; heap[pi] = tmp;
            ci = pi;
        }
    }

    public T Dequeue()
    {
        int li = heap.Count - 1;
        T frontItem = heap[0];
        heap[0] = heap[li];
        heap.RemoveAt(li);

        --li;
        int pi = 0;
        while (true)
        {
            int ci = pi * 2 + 1;
            if (ci > li) break;
            int rc = ci + 1;
            if (rc <= li && comparison(heap[rc], heap[ci]) < 0)
                ci = rc;
            if (comparison(heap[pi], heap[ci]) <= 0) break;
            T tmp = heap[pi]; heap[pi] = heap[ci]; heap[ci] = tmp;
            pi = ci;
        }
        return frontItem;
    }

    public T Peek() => heap[0];

    public bool Contains(T item) => heap.Contains(item);
}