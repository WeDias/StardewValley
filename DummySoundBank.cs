// Decompiled with JetBrains decompiler
// Type: StardewValley.DummySoundBank
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Audio;
using System;

namespace StardewValley
{
  public class DummySoundBank : ISoundBank, IDisposable
  {
    private ICue dummyCue = (ICue) new DummyCue();

    public bool IsInUse => false;

    public bool IsDisposed => true;

    public void Dispose()
    {
    }

    public ICue GetCue(string name) => this.dummyCue;

    public void PlayCue(string name)
    {
    }

    public void PlayCue(string name, AudioListener listener, AudioEmitter emitter)
    {
    }

    public void AddCue(CueDefinition cue_definition)
    {
    }

    public CueDefinition GetCueDefinition(string cue_name) => (CueDefinition) null;
  }
}
