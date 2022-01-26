// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.SwitchFloor
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class SwitchFloor : StardewValley.Object
  {
    public static Color successColor = Color.LightBlue;
    [XmlElement("onColor")]
    public readonly NetColor onColor = new NetColor();
    [XmlElement("offColor")]
    public readonly NetColor offColor = new NetColor();
    [XmlElement("readyToflip")]
    private readonly NetBool readyToflip = new NetBool(false);
    [XmlElement("finished")]
    public readonly NetBool finished = new NetBool(false);
    private int ticksToSuccess = -1;
    [XmlElement("glow")]
    private readonly NetFloat glow = new NetFloat(0.0f);

    public SwitchFloor() => this.NetFields.AddFields((INetSerializable) this.onColor, (INetSerializable) this.offColor, (INetSerializable) this.readyToflip, (INetSerializable) this.finished, (INetSerializable) this.glow);

    public SwitchFloor(Vector2 tileLocation, Color onColor, Color offColor, bool on)
      : this()
    {
      this.tileLocation.Value = tileLocation;
      this.onColor.Value = onColor;
      this.offColor.Value = offColor;
      this.isOn.Value = on;
      this.fragility.Value = 2;
      this.name = "Switch Floor";
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:SwitchFloor.cs.13097");

    public void flip(GameLocation environment)
    {
      this.isOn.Value = !(bool) (NetFieldBase<bool, NetBool>) this.isOn;
      this.glow.Value = 0.65f;
      foreach (Vector2 adjacentTileLocation in Utility.getAdjacentTileLocations((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation))
      {
        if (environment.objects.ContainsKey(adjacentTileLocation) && environment.objects[adjacentTileLocation] is SwitchFloor)
        {
          environment.objects[adjacentTileLocation].isOn.Value = !(bool) (NetFieldBase<bool, NetBool>) environment.objects[adjacentTileLocation].isOn;
          (environment.objects[adjacentTileLocation] as SwitchFloor).glow.Value = 0.3f;
        }
      }
      Game1.playSound("shiny4");
    }

    public void setSuccessCountdown(int ticks)
    {
      this.ticksToSuccess = ticks;
      this.glow.Value = 0.5f;
    }

    public void checkForCompleteness()
    {
      Queue<Vector2> vector2Queue = new Queue<Vector2>();
      HashSet<Vector2> source = new HashSet<Vector2>();
      vector2Queue.Enqueue((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation);
      Vector2 vector2_1 = new Vector2();
      List<Vector2> vector2List = new List<Vector2>();
      while (vector2Queue.Count > 0)
      {
        Vector2 vector2_2 = vector2Queue.Dequeue();
        if (Game1.currentLocation.objects.ContainsKey(vector2_2) && Game1.currentLocation.objects[vector2_2] is SwitchFloor && (NetFieldBase<bool, NetBool>) (Game1.currentLocation.objects[vector2_2] as SwitchFloor).isOn != this.isOn)
          return;
        source.Add(vector2_2);
        List<Vector2> adjacentTileLocations = Utility.getAdjacentTileLocations(vector2_2);
        for (int index = 0; index < adjacentTileLocations.Count; ++index)
        {
          if (!source.Contains(adjacentTileLocations[index]) && Game1.currentLocation.objects.ContainsKey(vector2_2) && Game1.currentLocation.objects[vector2_2] is SwitchFloor)
            vector2Queue.Enqueue(adjacentTileLocations[index]);
        }
        adjacentTileLocations.Clear();
      }
      int ticks = 5;
      foreach (Vector2 key in source)
      {
        if (Game1.currentLocation.objects.ContainsKey(key) && Game1.currentLocation.objects[key] is SwitchFloor)
          (Game1.currentLocation.objects[key] as SwitchFloor).setSuccessCountdown(ticks);
        ticks += 2;
      }
      int coins = (int) Math.Sqrt((double) source.Count) * 2;
      Vector2 vector2_3 = source.Last<Vector2>();
      while (Game1.currentLocation.isTileOccupiedByFarmer(vector2_3) != null)
      {
        source.Remove(vector2_3);
        if (source.Count > 0)
          vector2_3 = source.Last<Vector2>();
      }
      Game1.currentLocation.objects[vector2_3] = (StardewValley.Object) new Chest(coins, (List<Item>) null, vector2_3);
      Game1.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 320, 64, 64), 50f, 8, 0, vector2_3 * 64f, false, false));
      Game1.playSound("coin");
    }

    public override bool isPassable() => true;

    public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1f)
    {
      spriteBatch.Draw(Flooring.floorsTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64))), new Rectangle?(new Rectangle(0, 1280, 64, 64)), (bool) (NetFieldBase<bool, NetBool>) this.finished ? SwitchFloor.successColor : (Color) ((bool) (NetFieldBase<bool, NetBool>) this.isOn ? (NetFieldBase<Color, NetColor>) this.onColor : (NetFieldBase<Color, NetColor>) this.offColor), 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-08f);
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.glow <= 0.0)
        return;
      spriteBatch.Draw(Flooring.floorsTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((float) (x * 64), (float) (y * 64))), new Rectangle?(new Rectangle(0, 1280, 64, 64)), Color.White * (float) (NetFieldBase<float, NetFloat>) this.glow, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, 2E-08f);
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      if ((double) (float) (NetFieldBase<float, NetFloat>) this.glow > 0.0)
        this.glow.Value -= 0.04f;
      if (this.ticksToSuccess > 0)
      {
        --this.ticksToSuccess;
        if (this.ticksToSuccess != 0)
          return;
        this.finished.Value = true;
        this.glow.Value += 0.2f;
        Game1.playSound("boulderCrack");
      }
      else
      {
        if ((bool) (NetFieldBase<bool, NetBool>) this.finished)
          return;
        foreach (Character farmer in Game1.currentLocation.farmers)
        {
          if (farmer.getTileLocation().Equals((Vector2) (NetFieldBase<Vector2, NetVector2>) this.tileLocation))
          {
            if ((bool) (NetFieldBase<bool, NetBool>) this.readyToflip)
            {
              this.flip(Game1.currentLocation);
              this.checkForCompleteness();
            }
            this.readyToflip.Value = false;
            return;
          }
        }
        this.readyToflip.Value = true;
      }
    }
  }
}
