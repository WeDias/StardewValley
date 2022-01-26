// Decompiled with JetBrains decompiler
// Type: StardewValley.AudioEngineWrapper
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Audio;
using System;

namespace StardewValley
{
  internal class AudioEngineWrapper : IAudioEngine, IDisposable
  {
    private AudioEngine audioEngine;

    public AudioEngineWrapper(AudioEngine engine) => this.audioEngine = engine;

    public bool IsDisposed => this.audioEngine.IsDisposed;

    public void Dispose() => this.audioEngine.Dispose();

    public IAudioCategory GetCategory(string name) => (IAudioCategory) new AudioCategoryWrapper(this.audioEngine.GetCategory(name));

    public int GetCategoryIndex(string name) => this.audioEngine.GetCategoryIndex(name);

    public void Update() => this.audioEngine.Update();

    public AudioEngine Engine => this.audioEngine;
  }
}
