// Decompiled with JetBrains decompiler
// Type: StardewValley.NetSynchronizer
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;

namespace StardewValley
{
  public abstract class NetSynchronizer
  {
    private const byte MessageTypeVar = 0;
    private const byte MessageTypeBarrier = 1;
    private Dictionary<string, INetObject<INetSerializable>> variables = new Dictionary<string, INetObject<INetSerializable>>();
    private Dictionary<string, HashSet<long>> barriers = new Dictionary<string, HashSet<long>>();
    public Action<bool> barrierPoll;

    private HashSet<long> barrierPlayers(string name)
    {
      if (!this.barriers.ContainsKey(name))
        this.barriers[name] = new HashSet<long>();
      return this.barriers[name];
    }

    private bool barrierReady(string name)
    {
      HashSet<long> longSet = this.barrierPlayers(name);
      foreach (long key in (IEnumerable<long>) Game1.otherFarmers.Keys)
      {
        if (!longSet.Contains(key))
          return false;
      }
      return true;
    }

    private bool shouldAbort() => Game1.client != null && Game1.client.timedOut;

    public void barrier(string name)
    {
      this.barrierPlayers(name).Add(Game1.player.UniqueMultiplayerID);
      this.sendMessage((object) (byte) 1, (object) name);
      while (!this.barrierReady(name))
      {
        this.processMessages();
        if (this.shouldAbort())
          throw new AbortNetSynchronizerException();
        if (LocalMultiplayer.IsLocalMultiplayer())
          break;
      }
    }

    public bool isBarrierReady(string name)
    {
      if (this.barrierReady(name))
        return true;
      this.processMessages();
      if (this.shouldAbort())
        throw new AbortNetSynchronizerException();
      return false;
    }

    public bool isVarReady(string varName)
    {
      if (this.variables.ContainsKey(varName))
        return true;
      this.processMessages();
      if (this.shouldAbort())
        throw new AbortNetSynchronizerException();
      LocalMultiplayer.IsLocalMultiplayer();
      return false;
    }

    public T waitForVar<TField, T>(string varName) where TField : NetFieldBase<T, TField>, new()
    {
      while (!this.variables.ContainsKey(varName))
      {
        this.processMessages();
        if (this.shouldAbort())
          throw new AbortNetSynchronizerException();
      }
      return (this.variables[varName] as TField).Value;
    }

    public void sendVar<TField, T>(string varName, T value) where TField : NetFieldBase<T, TField>, new()
    {
      using (MemoryStream output = new MemoryStream())
      {
        using (BinaryWriter writer = new BinaryWriter((Stream) output))
        {
          NetRoot<TField> netRoot = new NetRoot<TField>(new TField());
          netRoot.Value.Value = value;
          netRoot.WriteFull(writer);
          this.variables[varName] = (INetObject<INetSerializable>) netRoot.Value;
          output.Seek(0L, SeekOrigin.Begin);
          this.sendMessage((object) (byte) 0, (object) varName, (object) output.ToArray());
        }
      }
    }

    public bool hasVar(string varName) => this.variables.ContainsKey(varName);

    public abstract void processMessages();

    protected abstract void sendMessage(params object[] data);

    public void receiveMessage(IncomingMessage msg)
    {
      switch (msg.Reader.ReadByte())
      {
        case 0:
          string key = msg.Reader.ReadString();
          NetRoot<INetObject<INetSerializable>> netRoot = new NetRoot<INetObject<INetSerializable>>();
          netRoot.ReadFull(msg.Reader, new NetVersion());
          this.variables[key] = netRoot.Value;
          break;
        case 1:
          this.barrierPlayers(msg.Reader.ReadString()).Add(msg.FarmerID);
          break;
      }
    }
  }
}
