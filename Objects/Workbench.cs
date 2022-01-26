// Decompiled with JetBrains decompiler
// Type: StardewValley.Objects.Workbench
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Objects
{
  public class Workbench : StardewValley.Object
  {
    [XmlIgnore]
    public readonly NetMutex mutex = new NetMutex();

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.mutex.NetFields);
    }

    public Workbench()
    {
    }

    public Workbench(Vector2 position)
      : base(position, 208)
    {
      this.Name = nameof (Workbench);
      this.type.Value = "Crafting";
      this.bigCraftable.Value = true;
      this.canBeSetDown.Value = true;
    }

    public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
    {
      if (justCheckingForActivity)
        return true;
      List<Chest> nearby_chests = new List<Chest>();
      Vector2[] vector2Array = new Vector2[8]
      {
        new Vector2(-1f, 1f),
        new Vector2(0.0f, 1f),
        new Vector2(1f, 1f),
        new Vector2(-1f, 0.0f),
        new Vector2(1f, 0.0f),
        new Vector2(-1f, -1f),
        new Vector2(0.0f, -1f),
        new Vector2(1f, -1f)
      };
      for (int index = 0; index < vector2Array.Length; ++index)
      {
        if (who.currentLocation is FarmHouse && who.currentLocation.getTileIndexAt((int) ((double) this.tileLocation.X + (double) vector2Array[index].X), (int) ((double) this.tileLocation.Y + (double) vector2Array[index].Y), "Buildings") == 173)
          nearby_chests.Add((who.currentLocation as FarmHouse).fridge.Value);
        else if (who.currentLocation is IslandFarmHouse && who.currentLocation.getTileIndexAt((int) ((double) this.tileLocation.X + (double) vector2Array[index].X), (int) ((double) this.tileLocation.Y + (double) vector2Array[index].Y), "Buildings") == 173)
        {
          nearby_chests.Add((who.currentLocation as IslandFarmHouse).fridge.Value);
        }
        else
        {
          Vector2 key = new Vector2((float) (int) ((double) this.tileLocation.X + (double) vector2Array[index].X), (float) (int) ((double) this.tileLocation.Y + (double) vector2Array[index].Y));
          StardewValley.Object @object = (StardewValley.Object) null;
          if (who.currentLocation.objects.ContainsKey(key))
            @object = who.currentLocation.objects[key];
          if (@object != null && @object is Chest && (@object as Chest).SpecialChestType == Chest.SpecialChestTypes.None)
            nearby_chests.Add(@object as Chest);
        }
      }
      List<NetMutex> mutexes = new List<NetMutex>();
      foreach (Chest chest in nearby_chests)
        mutexes.Add(chest.mutex);
      if (!this.mutex.IsLocked())
      {
        MultipleMutexRequest multipleMutexRequest = (MultipleMutexRequest) null;
        multipleMutexRequest = new MultipleMutexRequest(mutexes, (Action) (() => this.mutex.RequestLock((Action) (() =>
        {
          Vector2 centeringOnScreen = Utility.getTopLeftPositionForCenteringOnScreen(800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2);
          Game1.activeClickableMenu = (IClickableMenu) new CraftingPage((int) centeringOnScreen.X, (int) centeringOnScreen.Y, 800 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2, standalone_menu: true, material_containers: nearby_chests);
          Game1.activeClickableMenu.exitFunction = (IClickableMenu.onExit) (() =>
          {
            this.mutex.ReleaseLock();
            multipleMutexRequest.ReleaseLocks();
          });
        }), (Action) (() => multipleMutexRequest.ReleaseLocks()))), (Action) (() => Game1.showRedMessage(Game1.content.LoadString("Strings\\UI:Workbench_Chest_Warning"))));
      }
      return true;
    }

    public override void updateWhenCurrentLocation(GameTime time, GameLocation environment)
    {
      this.mutex.Update(environment);
      base.updateWhenCurrentLocation(time, environment);
    }
  }
}
