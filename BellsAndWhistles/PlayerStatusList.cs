// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.PlayerStatusList
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Network;
using System;
using System.Collections.Generic;

namespace StardewValley.BellsAndWhistles
{
  public class PlayerStatusList : INetObject<NetFields>
  {
    protected readonly NetLongDictionary<string, NetString> _statusList = new NetLongDictionary<string, NetString>();
    protected Dictionary<long, string> _formattedStatusList = new Dictionary<long, string>();
    protected Dictionary<string, Texture2D> _iconSprites = new Dictionary<string, Texture2D>();
    protected List<Farmer> _sortedFarmers = new List<Farmer>();
    public int iconAnimationFrames = 1;
    public int largestSpriteWidth;
    public int largestSpriteHeight;
    public PlayerStatusList.SortMode sortMode;
    public PlayerStatusList.DisplayMode displayMode;
    protected Dictionary<string, KeyValuePair<string, Rectangle>> _iconDefinitions = new Dictionary<string, KeyValuePair<string, Rectangle>>();

    public NetFields NetFields { get; } = new NetFields();

    public PlayerStatusList()
    {
      this.InitNetFields();
      this._iconDefinitions = new Dictionary<string, KeyValuePair<string, Rectangle>>();
      this._formattedStatusList = new Dictionary<long, string>();
    }

    public void InitNetFields()
    {
      this._statusList.InterpolationWait = false;
      this.NetFields.AddFields((INetSerializable) this._statusList);
      this._statusList.OnConflictResolve += (NetDictionary<long, string, NetString, SerializableDictionary<long, string>, NetLongDictionary<string, NetString>>.ConflictResolveEvent) ((a, b, c) => this._OnValueChanged());
      this._statusList.OnValueAdded += (NetDictionary<long, string, NetString, SerializableDictionary<long, string>, NetLongDictionary<string, NetString>>.ContentsChangeEvent) ((a, b) => this._OnValueChanged());
      this._statusList.OnValueRemoved += (NetDictionary<long, string, NetString, SerializableDictionary<long, string>, NetLongDictionary<string, NetString>>.ContentsChangeEvent) ((a, b) => this._OnValueChanged());
    }

    public void AddSpriteDefinition(
      string key,
      string file,
      int x,
      int y,
      int width,
      int height)
    {
      if (!this._iconSprites.ContainsKey(file) || this._iconSprites[file].IsDisposed)
        this._iconSprites[file] = Game1.content.Load<Texture2D>(file);
      this._iconDefinitions[key] = new KeyValuePair<string, Rectangle>(file, new Rectangle(x, y, width, height));
      if (width > this.largestSpriteWidth)
        this.largestSpriteWidth = width;
      if (height <= this.largestSpriteHeight)
        return;
      this.largestSpriteHeight = height;
    }

    public void UpdateState(string new_state)
    {
      if (this._statusList.ContainsKey(Game1.player.UniqueMultiplayerID) && !(this._statusList[Game1.player.UniqueMultiplayerID] != new_state))
        return;
      this._statusList.Remove(Game1.player.UniqueMultiplayerID);
      this._statusList.Add(Game1.player.UniqueMultiplayerID, new_state);
    }

    public void WithdrawState()
    {
      if (!this._statusList.ContainsKey(Game1.player.UniqueMultiplayerID))
        return;
      this._statusList.Remove(Game1.player.UniqueMultiplayerID);
    }

    protected void _OnValueChanged()
    {
      foreach (long key in this._statusList.Keys)
        this._formattedStatusList[key] = this.GetStatusText(key);
      this._ResortList();
    }

    protected void _ResortList()
    {
      this._sortedFarmers.Clear();
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
        this._sortedFarmers.Add(onlineFarmer);
      foreach (Farmer allFarmer in Game1.getAllFarmers())
      {
        if (Game1.IsMasterGame && !this._sortedFarmers.Contains(allFarmer) && this._statusList.ContainsKey(allFarmer.UniqueMultiplayerID))
          this._statusList.Remove(allFarmer.UniqueMultiplayerID);
        if (!this._statusList.ContainsKey(allFarmer.UniqueMultiplayerID))
          this._sortedFarmers.Remove(allFarmer);
      }
      if (this.sortMode == PlayerStatusList.SortMode.AlphaSort || this.sortMode == PlayerStatusList.SortMode.AlphaSortDescending)
      {
        this._sortedFarmers.Sort((Comparison<Farmer>) ((a, b) => this.GetStatusText(a.UniqueMultiplayerID).CompareTo(this.GetStatusText(b.UniqueMultiplayerID))));
        if (this.sortMode != PlayerStatusList.SortMode.AlphaSortDescending)
          return;
        this._sortedFarmers.Reverse();
      }
      else
      {
        if (this.sortMode != PlayerStatusList.SortMode.NumberSort && this.sortMode != PlayerStatusList.SortMode.NumberSortDescending)
          return;
        this._sortedFarmers.Sort((Comparison<Farmer>) ((a, b) => int.Parse(this.GetStatusText(a.UniqueMultiplayerID)).CompareTo(int.Parse(this.GetStatusText(b.UniqueMultiplayerID)))));
        if (this.sortMode != PlayerStatusList.SortMode.NumberSortDescending)
          return;
        this._sortedFarmers.Reverse();
      }
    }

    public string GetStatusText(long id)
    {
      if (!this._statusList.ContainsKey(id))
        return "";
      return this.displayMode == PlayerStatusList.DisplayMode.LocalizedText ? Game1.content.LoadString(this._statusList[id]) : this._statusList[id];
    }

    public void Draw(
      SpriteBatch b,
      Vector2 draw_position,
      float draw_scale = 4f,
      float draw_layer = 0.45f,
      PlayerStatusList.HorizontalAlignment horizontal_origin = PlayerStatusList.HorizontalAlignment.Left,
      PlayerStatusList.VerticalAlignment vertical_origin = PlayerStatusList.VerticalAlignment.Top)
    {
      float num1 = 12f;
      if (this.displayMode == PlayerStatusList.DisplayMode.Icons && (double) this.largestSpriteHeight > (double) num1)
        num1 = (float) this.largestSpriteHeight;
      if (horizontal_origin == PlayerStatusList.HorizontalAlignment.Right)
      {
        float num2 = 0.0f;
        if (this.displayMode == PlayerStatusList.DisplayMode.Icons)
        {
          draw_position.X -= (float) this.largestSpriteWidth * draw_scale;
        }
        else
        {
          foreach (Farmer sortedFarmer in this._sortedFarmers)
          {
            if (this._formattedStatusList.ContainsKey(sortedFarmer.UniqueMultiplayerID))
            {
              float x = Game1.dialogueFont.MeasureString(this._formattedStatusList[sortedFarmer.UniqueMultiplayerID]).X;
              if ((double) num2 < (double) x)
                num2 = x;
            }
          }
          draw_position.X -= (num2 + 16f) * draw_scale;
        }
      }
      if (vertical_origin == PlayerStatusList.VerticalAlignment.Bottom)
        draw_position.Y -= num1 * (float) this._statusList.Count() * draw_scale;
      foreach (Farmer sortedFarmer in this._sortedFarmers)
      {
        float num3 = Game1.isUsingBackToFrontSorting ? -1f : 1f;
        if (this._formattedStatusList.ContainsKey(sortedFarmer.UniqueMultiplayerID))
        {
          Vector2 zero = Vector2.Zero;
          sortedFarmer.FarmerRenderer.drawMiniPortrat(b, draw_position, draw_layer, draw_scale * 0.75f, 2, sortedFarmer);
          if (this.displayMode == PlayerStatusList.DisplayMode.Icons && this._iconDefinitions.ContainsKey(this._formattedStatusList[sortedFarmer.UniqueMultiplayerID]))
          {
            zero.X += 12f * draw_scale;
            KeyValuePair<string, Rectangle> iconDefinition = this._iconDefinitions[this._formattedStatusList[sortedFarmer.UniqueMultiplayerID]];
            Rectangle rectangle = iconDefinition.Value with
            {
              Y = (int) (Game1.currentGameTime.TotalGameTime.TotalMilliseconds % (double) (this.iconAnimationFrames * 100) / 100.0) * 16
            };
            b.Draw(this._iconSprites[iconDefinition.Key], draw_position + zero, new Rectangle?(rectangle), Color.White, 0.0f, Vector2.Zero, draw_scale, SpriteEffects.None, draw_layer - 0.0001f * num3);
          }
          else
          {
            zero.X += 16f * draw_scale;
            zero.Y += 2f * draw_scale;
            string formattedStatus = this._formattedStatusList[sortedFarmer.UniqueMultiplayerID];
            b.DrawString(Game1.dialogueFont, formattedStatus, draw_position + zero + Vector2.One * draw_scale, Color.Black, 0.0f, Vector2.Zero, draw_scale / 4f, SpriteEffects.None, draw_layer - 0.0001f * num3);
            b.DrawString(Game1.dialogueFont, formattedStatus, draw_position + zero, Color.White, 0.0f, Vector2.Zero, draw_scale / 4f, SpriteEffects.None, draw_layer);
          }
          draw_position.Y += num1 * draw_scale;
        }
      }
    }

    public enum SortMode
    {
      None,
      NumberSort,
      NumberSortDescending,
      AlphaSort,
      AlphaSortDescending,
    }

    public enum DisplayMode
    {
      Text,
      LocalizedText,
      Icons,
    }

    public enum VerticalAlignment
    {
      Top,
      Bottom,
    }

    public enum HorizontalAlignment
    {
      Left,
      Right,
    }
  }
}
