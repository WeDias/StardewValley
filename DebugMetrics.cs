// Decompiled with JetBrains decompiler
// Type: StardewValley.DebugMetricsComponent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace StardewValley
{
  public class DebugMetricsComponent : DrawableGameComponent
  {
    private readonly Game _game;
    private SpriteFont _font;
    private SpriteBatch _spriteBatch;
    private int _drawX;
    private int _drawY;
    private double _fps;
    private double _mspf;
    private int _lastCollection;
    private float _lastBaseMB;
    private bool _runningSlowly;
    private StringBuilder _stringBuilder = new StringBuilder(512);
    private Texture2D _opaqueWhite;
    public int XOffset = 10;
    public int YOffset = 10;
    private IBandwidthMonitor bandwidthMonitor;
    private BarGraph bandwidthUpGraph;
    private BarGraph bandwidthDownGraph;

    public SpriteFont Font
    {
      get => this._font;
      set => this._font = value;
    }

    public DebugMetricsComponent(Game game)
      : base(game)
    {
      this._game = game;
      this.DrawOrder = int.MaxValue;
    }

    protected override void LoadContent()
    {
      this._spriteBatch = new SpriteBatch(this.GraphicsDevice);
      int width = 2;
      int height = 2;
      this._opaqueWhite = new Texture2D(this.GraphicsDevice, width, height, false, SurfaceFormat.Color);
      Color[] data = new Color[width * height];
      this._opaqueWhite.GetData<Color>(data);
      for (int index = 0; index < width * height; ++index)
        data[index] = Color.White;
      this._opaqueWhite.SetData<Color>(data);
      base.LoadContent();
    }

    public override void Update(GameTime gameTime)
    {
      this.bandwidthMonitor = !Game1.IsServer ? (!Game1.IsClient ? (IBandwidthMonitor) null : (IBandwidthMonitor) Game1.client) : (IBandwidthMonitor) Game1.server;
      if (this.bandwidthMonitor == null || !this.bandwidthMonitor.LogBandwidth)
      {
        this.bandwidthDownGraph = (BarGraph) null;
        this.bandwidthUpGraph = (BarGraph) null;
      }
      if (this.bandwidthMonitor == null || !this.bandwidthMonitor.LogBandwidth || this.bandwidthDownGraph != null && this.bandwidthUpGraph != null)
        return;
      int width = 200;
      int height = 150;
      int y = 50;
      this.bandwidthUpGraph = new BarGraph(this.bandwidthMonitor.BandwidthLogger.LoggedAvgBitsUp, Game1.uiViewport.Width - width - y, y, width, height, 2, BarGraph.DYNAMIC_SCALE_MAX, Color.Yellow * 0.8f, this._opaqueWhite);
      this.bandwidthDownGraph = new BarGraph(this.bandwidthMonitor.BandwidthLogger.LoggedAvgBitsDown, Game1.uiViewport.Width - width - y, y + height + y, width, height, 2, BarGraph.DYNAMIC_SCALE_MAX, Color.Cyan * 0.8f, this._opaqueWhite);
    }

    public override void Draw(GameTime gameTime)
    {
      if (!Game1.displayHUD || !Game1.debugMode)
        return;
      TimeSpan elapsedGameTime = gameTime.ElapsedGameTime;
      if (elapsedGameTime.TotalSeconds > 0.0)
      {
        elapsedGameTime = gameTime.ElapsedGameTime;
        this._fps = 1.0 / elapsedGameTime.TotalSeconds;
        elapsedGameTime = gameTime.ElapsedGameTime;
        this._mspf = elapsedGameTime.TotalSeconds * 1000.0;
      }
      if (gameTime.IsRunningSlowly)
        this._runningSlowly = true;
      if (this._font == null)
        return;
      this._spriteBatch.Begin();
      this._drawX = this.XOffset;
      this._drawY = this.YOffset;
      StringBuilder stringBuilder = this._stringBuilder;
      Utility.makeSafe(ref this._drawX, ref this._drawY, 0, 0);
      int num1 = GC.CollectionCount(0);
      float num2 = (float) GC.GetTotalMemory(false) / 1048576f;
      if (this._lastCollection != num1)
      {
        this._lastCollection = num1;
        this._lastBaseMB = num2;
      }
      float num3 = num2 - this._lastBaseMB;
      stringBuilder.AppendFormatEx<int, int, float, float>("FPS: {0,3}   GC: {1,3}   {2:0.00}MB   +{3:0.00}MB", (int) Math.Round(this._fps), this._lastCollection % 1000, this._lastBaseMB, num3);
      Color color = Color.Yellow;
      if (this._runningSlowly)
      {
        stringBuilder.Append("   [IsRunningSlowly]");
        this._runningSlowly = false;
        color = Color.Red;
      }
      this.DrawLine(color, stringBuilder, this._drawX);
      if (Game1.IsMultiplayer)
      {
        color = Color.Yellow;
        if (Game1.IsServer)
        {
          foreach (KeyValuePair<long, Farmer> otherFarmer in Game1.otherFarmers)
          {
            stringBuilder.AppendFormat("Ping({0}): {1:0.0}ms", (object) otherFarmer.Value.Name, (object) Game1.server.getPingToClient(otherFarmer.Key));
            this.DrawLine(color, stringBuilder, this._drawX);
          }
        }
        else
        {
          stringBuilder.AppendFormat("Ping: {0:0.0}ms", (object) Game1.client.GetPingToHost());
          this.DrawLine(color, stringBuilder, this._drawX);
        }
      }
      if (this.bandwidthMonitor != null && this.bandwidthMonitor.LogBandwidth)
      {
        stringBuilder.AppendFormat("Up - b/s: {0}  Avg b/s: {1}", (object) (int) this.bandwidthMonitor.BandwidthLogger.BitsUpPerSecond, (object) (int) this.bandwidthMonitor.BandwidthLogger.AvgBitsUpPerSecond);
        this.DrawLine(color, stringBuilder, this._drawX);
        stringBuilder.AppendFormat("Down - b/s: {0}  Avg b/s: {1}", (object) (int) this.bandwidthMonitor.BandwidthLogger.BitsDownPerSecond, (object) (int) this.bandwidthMonitor.BandwidthLogger.AvgBitsDownPerSecond);
        this.DrawLine(color, stringBuilder, this._drawX);
        stringBuilder.AppendFormat("Total MB Up: {0:0.00}  Total MB Down: {1:0.00}  Total Seconds: {2:0.00}", (object) (float) (this.bandwidthMonitor.BandwidthLogger.TotalBitsUp / 8.0 / 1000.0 / 1000.0), (object) (float) (this.bandwidthMonitor.BandwidthLogger.TotalBitsDown / 8.0 / 1000.0 / 1000.0), (object) (float) (this.bandwidthMonitor.BandwidthLogger.TotalMs / 1000.0));
        this.DrawLine(color, stringBuilder, this._drawX);
        if (this.bandwidthUpGraph != null && this.bandwidthDownGraph != null)
        {
          this.bandwidthUpGraph.Draw(this._spriteBatch);
          this.bandwidthDownGraph.Draw(this._spriteBatch);
        }
      }
      this._spriteBatch.End();
    }

    private void DrawLine(Color color, StringBuilder sb, int x)
    {
      if (sb == null)
        return;
      Vector2 vector2 = this._font.MeasureString(sb);
      int drawY = this._drawY;
      int lineSpacing = this._font.LineSpacing;
      int height = lineSpacing - lineSpacing / 10;
      this._spriteBatch.Draw(this._opaqueWhite, new Rectangle(x - 1, drawY, (int) vector2.X + 2, height), new Rectangle?(), Color.Black * 0.5f);
      this._spriteBatch.DrawString(this._font, sb, new Vector2((float) x, (float) drawY), color);
      this._drawY += height;
      sb.Clear();
    }
  }
}
