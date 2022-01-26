// Decompiled with JetBrains decompiler
// Type: StardewValley.PriorityQueue
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System.Collections.Generic;

namespace StardewValley
{
  public class PriorityQueue
  {
    private int total_size;
    private SortedDictionary<int, Queue<PathNode>> nodes;

    public PriorityQueue()
    {
      this.nodes = new SortedDictionary<int, Queue<PathNode>>();
      this.total_size = 0;
    }

    public bool IsEmpty() => this.total_size == 0;

    public void Clear()
    {
      this.total_size = 0;
      foreach (KeyValuePair<int, Queue<PathNode>> node in this.nodes)
        node.Value.Clear();
    }

    public bool Contains(PathNode p, int priority)
    {
      Queue<PathNode> pathNodeQueue;
      return this.nodes.TryGetValue(priority, out pathNodeQueue) && pathNodeQueue.Contains(p);
    }

    public PathNode Dequeue()
    {
      if (!this.IsEmpty())
      {
        foreach (Queue<PathNode> pathNodeQueue in this.nodes.Values)
        {
          if (pathNodeQueue.Count > 0)
          {
            --this.total_size;
            return pathNodeQueue.Dequeue();
          }
        }
      }
      return (PathNode) null;
    }

    public object Peek()
    {
      if (!this.IsEmpty())
      {
        foreach (Queue<PathNode> pathNodeQueue in this.nodes.Values)
        {
          if (pathNodeQueue.Count > 0)
            return (object) pathNodeQueue.Peek();
        }
      }
      return (object) null;
    }

    public object Dequeue(int priority)
    {
      --this.total_size;
      return (object) this.nodes[priority].Dequeue();
    }

    public void Enqueue(PathNode item, int priority)
    {
      if (!this.nodes.ContainsKey(priority))
      {
        this.nodes.Add(priority, new Queue<PathNode>());
        this.Enqueue(item, priority);
      }
      else
      {
        this.nodes[priority].Enqueue(item);
        ++this.total_size;
      }
    }
  }
}
