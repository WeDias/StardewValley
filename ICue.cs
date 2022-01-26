// Decompiled with JetBrains decompiler
// Type: StardewValley.ICue
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Audio;
using System;

namespace StardewValley
{
  public interface ICue : IDisposable
  {
    void Play();

    void Pause();

    void Resume();

    void Stop(AudioStopOptions options);

    void SetVariable(string var, int val);

    void SetVariable(string var, float val);

    float GetVariable(string var);

    bool IsStopped { get; }

    bool IsStopping { get; }

    bool IsPlaying { get; }

    bool IsPaused { get; }

    string Name { get; }

    float Pitch { get; set; }

    float Volume { get; set; }

    bool IsPitchBeingControlledByRPC { get; }
  }
}
