// Decompiled with JetBrains decompiler
// Type: StardewValley.DummyAudioEngine
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Audio;
using System;

namespace StardewValley
{
  internal class DummyAudioEngine : IAudioEngine, IDisposable
  {
    private IAudioCategory category = (IAudioCategory) new DummyAudioCategory();

    public void Update()
    {
    }

    public bool IsDisposed => true;

    public IAudioCategory GetCategory(string name) => this.category;

    public int GetCategoryIndex(string name) => -1;

    public void Dispose()
    {
    }

    public AudioEngine Engine => (AudioEngine) null;
  }
}
