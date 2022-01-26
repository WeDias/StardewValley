// Decompiled with JetBrains decompiler
// Type: StardewValley.InstanceGame
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace StardewValley
{
  public class InstanceGame
  {
    public object staticVarHolder;

    public bool IsMainInstance => GameRunner.instance.gameInstances.Count == 0 || GameRunner.instance.gameInstances[0] == this;

    protected virtual void Initialize()
    {
    }

    protected virtual void LoadContent()
    {
    }

    protected virtual void UnloadContent()
    {
    }

    protected virtual void Update(GameTime game_time)
    {
    }

    protected virtual void OnActivated(object sender, EventArgs args)
    {
    }

    protected virtual void Draw(GameTime game_time)
    {
    }

    public GraphicsDevice GraphicsDevice => GameRunner.instance.GraphicsDevice;

    public ContentManager Content => GameRunner.instance.Content;

    public GameComponentCollection Components => GameRunner.instance.Components;

    public GameWindow Window => GameRunner.instance.Window;

    public bool IsFixedTimeStep
    {
      get => GameRunner.instance.IsFixedTimeStep;
      set => GameRunner.instance.IsFixedTimeStep = value;
    }

    public bool IsActive => GameRunner.instance.IsActive;

    public bool IsMouseVisible
    {
      get => GameRunner.instance.IsMouseVisible;
      set => GameRunner.instance.IsMouseVisible = value;
    }

    protected virtual void BeginDraw()
    {
    }

    protected virtual void EndDraw()
    {
    }

    public void Exit() => GameRunner.instance.Exit();

    public TimeSpan TargetElapsedTime
    {
      get => GameRunner.instance.TargetElapsedTime;
      set => GameRunner.instance.TargetElapsedTime = value;
    }
  }
}
