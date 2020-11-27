﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Notung.Data
{
  /// <summary>
  /// Взвешенный граф, хранимый в виде списка смежных вершин
  /// </summary>
  /// <typeparam name="T">Тип веса вершины</typeparam>
  [Serializable]
  public class WeightedNestedList<T> : IWeightedGraph<T> where T : IConvertible
  {
    private readonly Dictionary<int, T>[] m_forward;
    private readonly Dictionary<int, T>[] m_reverse;

    /// <summary>
    /// Создание нового графа, хранимого в виде списка смежных вершин
    /// </summary>
    /// <param name="peakCount">Количество вершин графа</param>
    /// <param name="isOriented">Будет ли граф ориентированным</param>
    public WeightedNestedList(int peakCount, bool isOriented)
    {
      m_forward = ArrayExtensions.CreateAndFill(peakCount, () => new Dictionary<int, T>());

      if (isOriented)
        m_reverse = ArrayExtensions.CreateAndFill(peakCount, () => new Dictionary<int, T>());
    }

    public int PeakCount
    {
      get { return m_forward.Length; }
    }

    public bool IsOriented
    {
      get { return m_reverse != null; }
    }

    public void AddArc(int from, int to, T weight)
    {
      if (this.HasArc(from, to))
        throw new ArgumentException("Duplicate arc");

      m_forward[from].Add(to, weight);
      (m_reverse ?? m_forward)[to][from] = weight;
    }

    public bool HasArc(int from, int to)
    {
      if (to < 0 || to >= m_forward.Length)
        throw new IndexOutOfRangeException();

      if (from == to)
        throw new ArgumentException("from == to");

      return m_forward[from].ContainsKey(to);
    }

    public bool RemoveArc(int from, int to)
    {
      if (!this.HasArc(from, to))
        return false;

      m_forward[from].Remove(to);
      (m_reverse ?? m_forward)[to].Remove(from);

      return true;
    }

    public T GetWeight(int from, int to)
    {
      return m_forward[from][to];
    }

    public int IncomingCount(int peak)
    {
      return (m_reverse ?? m_forward)[peak].Count;
    }

    public int OutGoingCount(int peak)
    {
      return m_forward[peak].Count;
    }

    public IEnumerable<Tuple<int, T>> IncomingArcs(int peak)
    {
      return (m_reverse ?? m_forward)[peak].Select(kv => new Tuple<int, T>(kv.Key, kv.Value));
    }

    public IEnumerable<Tuple<int, T>> OutgoingArcs(int peak)
    {
      return m_forward[peak].Select(kv => new Tuple<int, T>(kv.Key, kv.Value));
    }
  }
}