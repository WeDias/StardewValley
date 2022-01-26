// Decompiled with JetBrains decompiler
// Type: StardewValley.ISoundBank
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Audio;
using System;

namespace StardewValley
{
  public interface ISoundBank : IDisposable
  {
    bool IsInUse { get; }

    ICue GetCue(string name);

    void PlayCue(string name);

    void PlayCue(string name, AudioListener listener, AudioEmitter emitter);

    void AddCue(CueDefinition cue_definition);

    CueDefinition GetCueDefinition(string name);

    bool IsDisposed { get; }
  }
}
