// Decompiled with JetBrains decompiler
// Type: StardewValley.CueWrapper
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Audio;
using System;

namespace StardewValley
{
  public class CueWrapper : ICue, IDisposable
  {
    private Cue cue;

    public CueWrapper(Cue cue) => this.cue = cue;

    public void Play() => this.cue.Play();

    public void Pause() => this.cue.Pause();

    public void Resume() => this.cue.Resume();

    public void Stop(AudioStopOptions options) => this.cue.Stop(options);

    public void SetVariable(string var, int val) => this.cue.SetVariable(var, (float) val);

    public void SetVariable(string var, float val) => this.cue.SetVariable(var, val);

    public float GetVariable(string var) => this.cue.GetVariable(var);

    public bool IsStopped => this.cue.IsStopped;

    public bool IsStopping => this.cue.IsStopping;

    public bool IsPlaying => this.cue.IsPlaying;

    public bool IsPaused => this.cue.IsPaused;

    public string Name => this.cue.Name;

    public void Dispose()
    {
      this.cue.Dispose();
      this.cue = (Cue) null;
    }

    public float Volume
    {
      get => this.cue.Volume;
      set => this.cue.Volume = value;
    }

    public float Pitch
    {
      get => this.cue.Pitch;
      set => this.cue.Pitch = value;
    }

    public bool IsPitchBeingControlledByRPC => this.cue.IsPitchBeingControlledByRPC;
  }
}
