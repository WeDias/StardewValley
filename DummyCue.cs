// Decompiled with JetBrains decompiler
// Type: StardewValley.DummyCue
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Audio;
using System;

namespace StardewValley
{
  public class DummyCue : ICue, IDisposable
  {
    public void Play()
    {
    }

    public void Pause()
    {
    }

    public void Resume()
    {
    }

    public void SetVariable(string var, int val)
    {
    }

    public void SetVariable(string var, float val)
    {
    }

    public float GetVariable(string var) => 0.0f;

    public bool IsStopped => true;

    public bool IsStopping => false;

    public bool IsPlaying => false;

    public bool IsPaused => false;

    public string Name => "";

    public void Stop(AudioStopOptions options)
    {
    }

    public void Dispose()
    {
    }

    public float Volume
    {
      get => 1f;
      set
      {
      }
    }

    public float Pitch
    {
      get => 0.0f;
      set
      {
      }
    }

    public bool IsPitchBeingControlledByRPC => true;
  }
}
