// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.MilkPail
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Netcode;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace StardewValley.Tools
{
  public class MilkPail : Tool
  {
    [XmlIgnore]
    private readonly NetEvent0 finishEvent = new NetEvent0();
    private FarmAnimal animal;

    public MilkPail()
      : base("Milk Pail", -1, 6, 6, false)
    {
    }

    public override Item getOne()
    {
      MilkPail destination = new MilkPail();
      this.CopyEnchantments((Tool) this, (Tool) destination);
      destination._GetOneFrom((Item) this);
      return (Item) destination;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:MilkPail.cs.14167");

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:MilkPail.cs.14168");

    protected override void initNetFields()
    {
      base.initNetFields();
      this.NetFields.AddFields((INetSerializable) this.finishEvent);
      this.finishEvent.onEvent += new NetEvent0.Event(this.doFinish);
    }

    public override bool beginUsing(GameLocation location, int x, int y, Farmer who)
    {
      x = (int) who.GetToolLocation().X;
      y = (int) who.GetToolLocation().Y;
      Rectangle toolRect = new Rectangle(x - 32, y - 32, 64, 64);
      if (location is Farm)
        this.animal = Utility.GetBestHarvestableFarmAnimal((IEnumerable<FarmAnimal>) (location as Farm).animals.Values, (Tool) this, toolRect);
      else if (location is AnimalHouse)
        this.animal = Utility.GetBestHarvestableFarmAnimal((IEnumerable<FarmAnimal>) (location as AnimalHouse).animals.Values, (Tool) this, toolRect);
      if (this.animal != null && (int) (NetFieldBase<int, NetInt>) this.animal.currentProduce > 0 && (int) (NetFieldBase<int, NetInt>) this.animal.age >= (int) (byte) (NetFieldBase<byte, NetByte>) this.animal.ageWhenMature && this.animal.toolUsedForHarvest.Equals((object) this.BaseName) && who.couldInventoryAcceptThisObject((int) (NetFieldBase<int, NetInt>) this.animal.currentProduce, 1))
      {
        this.animal.doEmote(20);
        this.animal.friendshipTowardFarmer.Value = Math.Min(1000, (int) (NetFieldBase<int, NetInt>) this.animal.friendshipTowardFarmer + 5);
        who.currentLocation.localSound("Milking");
        this.animal.pauseTimer = 1500;
      }
      else if (this.animal != null && (int) (NetFieldBase<int, NetInt>) this.animal.currentProduce > 0 && (int) (NetFieldBase<int, NetInt>) this.animal.age >= (int) (byte) (NetFieldBase<byte, NetByte>) this.animal.ageWhenMature)
      {
        if (who != null && Game1.player.Equals((object) who))
        {
          if (!this.animal.toolUsedForHarvest.Equals((object) this.BaseName))
          {
            if (!((NetFieldBase<string, NetString>) this.animal.toolUsedForHarvest == (NetString) null) && !this.animal.toolUsedForHarvest.Equals((object) "null"))
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:MilkPail.cs.14167", (object) this.animal.toolUsedForHarvest));
          }
          else if (!who.couldInventoryAcceptThisObject((int) (NetFieldBase<int, NetInt>) this.animal.currentProduce, 1))
            Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
        }
      }
      else if (who != null && Game1.player.Equals((object) who))
      {
        DelayedAction.playSoundAfterDelay("fishingRodBend", 300);
        DelayedAction.playSoundAfterDelay("fishingRodBend", 1200);
        string dialogue = "";
        if (this.animal != null && !this.animal.toolUsedForHarvest.Equals((object) this.BaseName))
          dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:MilkPail.cs.14175", (object) this.animal.displayName);
        if (this.animal != null && this.animal.isBaby() && this.animal.toolUsedForHarvest.Equals((object) this.BaseName))
          dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:MilkPail.cs.14176", (object) this.animal.displayName);
        if (this.animal != null && (int) (NetFieldBase<int, NetInt>) this.animal.age >= (int) (byte) (NetFieldBase<byte, NetByte>) this.animal.ageWhenMature && this.animal.toolUsedForHarvest.Equals((object) this.BaseName))
          dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:MilkPail.cs.14177", (object) this.animal.displayName);
        if (dialogue.Length > 0)
          DelayedAction.showDialogueAfterDelay(dialogue, 1000);
      }
      who.Halt();
      int currentFrame = who.FarmerSprite.CurrentFrame;
      who.FarmerSprite.animateOnce(287 + who.FacingDirection, 50f, 4);
      who.FarmerSprite.oldFrame = currentFrame;
      who.UsingTool = true;
      who.CanMove = false;
      return true;
    }

    public override void tickUpdate(GameTime time, Farmer who)
    {
      this.lastUser = who;
      base.tickUpdate(time, who);
      this.finishEvent.Poll();
    }

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      base.DoFunction(location, x, y, power, who);
      who.Stamina -= 4f;
      this.CurrentParentTileIndex = 6;
      this.IndexOfMenuItemView = 6;
      if (this.animal != null && (int) (NetFieldBase<int, NetInt>) this.animal.currentProduce > 0 && (int) (NetFieldBase<int, NetInt>) this.animal.age >= (int) (byte) (NetFieldBase<byte, NetByte>) this.animal.ageWhenMature && this.animal.toolUsedForHarvest.Equals((object) this.BaseName))
      {
        if (who.addItemToInventoryBool((Item) new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.animal.currentProduce, (string) null, false, true, false, false)
        {
          Quality = (int) (NetFieldBase<int, NetInt>) this.animal.produceQuality
        }))
        {
          Utility.RecordAnimalProduce(this.animal, (int) (NetFieldBase<int, NetInt>) this.animal.currentProduce);
          Game1.playSound("coin");
          this.animal.currentProduce.Value = -1;
          if ((bool) (NetFieldBase<bool, NetBool>) this.animal.showDifferentTextureWhenReadyForHarvest)
            this.animal.Sprite.LoadTexture("Animals\\Sheared" + this.animal.type.Value);
          who.gainExperience(0, 5);
        }
      }
      this.finish();
    }

    private void finish() => this.finishEvent.Fire();

    private void doFinish()
    {
      this.animal = (FarmAnimal) null;
      this.lastUser.CanMove = true;
      this.lastUser.completelyStopAnimatingOrDoingAction();
      this.lastUser.UsingTool = false;
      this.lastUser.canReleaseTool = true;
    }
  }
}
