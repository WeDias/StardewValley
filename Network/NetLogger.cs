// Decompiled with JetBrains decompiler
// Type: StardewValley.NetLogger
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace StardewValley
{
  public class NetLogger
  {
    private Dictionary<string, NetLogRecord> loggedWrites = new Dictionary<string, NetLogRecord>();
    private DateTime timeLastStarted;
    private double priorMillis;
    private bool isLogging;

    public bool IsLogging
    {
      get => this.isLogging;
      set
      {
        if (value == this.isLogging)
          return;
        this.isLogging = value;
        if (this.isLogging)
          this.timeLastStarted = DateTime.Now;
        else
          this.priorMillis += (DateTime.Now - this.timeLastStarted).TotalMilliseconds;
      }
    }

    public double LogDuration => this.isLogging ? this.priorMillis + (DateTime.Now - this.timeLastStarted).TotalMilliseconds : this.priorMillis;

    public void LogWrite(string path, long length)
    {
      if (!this.IsLogging)
        return;
      NetLogRecord netLogRecord;
      this.loggedWrites.TryGetValue(path, out netLogRecord);
      netLogRecord.Path = path;
      ++netLogRecord.Count;
      netLogRecord.Bytes += length;
      this.loggedWrites[path] = netLogRecord;
    }

    public void Clear()
    {
      this.loggedWrites.Clear();
      this.priorMillis = 0.0;
      this.timeLastStarted = DateTime.Now;
    }

    public string Dump()
    {
      string str = Path.Combine(Environment.GetFolderPath(Environment.OSVersion.Platform != PlatformID.Unix ? Environment.SpecialFolder.ApplicationData : Environment.SpecialFolder.LocalApplicationData), "StardewValley", "Profiling", DateTime.Now.Ticks.ToString() + ".csv");
      FileInfo fileInfo = new FileInfo(str);
      if (!fileInfo.Directory.Exists)
        fileInfo.Directory.Create();
      using (StreamWriter text = File.CreateText(str))
      {
        double num = this.LogDuration / 1000.0;
        text.WriteLine("Profile Duration: {0:F2}", (object) num);
        text.WriteLine("Stack,Deltas,Bytes,Deltas/s,Bytes/s,Bytes/Delta");
        foreach (NetLogRecord netLogRecord in this.loggedWrites.Values)
          text.WriteLine("{0:F2},{1:F2},{2:F2},{3:F2},{4:F2},{5:F2}", (object) netLogRecord.Path, (object) netLogRecord.Count, (object) netLogRecord.Bytes, (object) ((double) netLogRecord.Count / num), (object) ((double) netLogRecord.Bytes / num), (object) ((double) netLogRecord.Bytes / (double) netLogRecord.Count));
      }
      return str;
    }
  }
}
