﻿// Decompiled with JetBrains decompiler
// Type: StardewValley.IAudioEngine
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Audio;
using System;

namespace StardewValley
{
  public interface IAudioEngine : IDisposable
  {
    void Update();

    bool IsDisposed { get; }

    IAudioCategory GetCategory(string name);

    int GetCategoryIndex(string name);

    AudioEngine Engine { get; }
  }
}
