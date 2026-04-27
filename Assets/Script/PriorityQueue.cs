using UnityEngine;

using System;
using System.Collections.Generic;

public class PriorityQueue<TElement, TPriority> where TPriority : IComparable<TPriority>
{
    private List<(TElement Element, TPriority Priority)> _elements = new List<(TElement, TPriority)>();

    public int Count => _elements.Count;

    public void Enqueue(TElement element, TPriority priority)
    {
        _elements.Add((element, priority));
    }

    public TElement Dequeue()
    {
        if (_elements.Count == 0) throw new InvalidOperationException("Queue is empty");

        int bestIndex = 0;
        for (int i = 1; i < _elements.Count; i++)
        {
            if (_elements[i].Priority.CompareTo(_elements[bestIndex].Priority) < 0)
            {
                bestIndex = i;
            }
        }

        TElement bestElement = _elements[bestIndex].Element;
        _elements.RemoveAt(bestIndex);
        return bestElement;
    }
}
