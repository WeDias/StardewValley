// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Shears
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
  public class Shears : Tool
  {
    [XmlIgnore]
    private readonly NetEvent0 finishEvent = new NetEvent0();
    private FarmAnimal animal;

    public Shears()
      : base(nameof (Shears), -1, 7, 7, false)
    {
    }

    public override Item getOne()
    {
      Shears destination = new Shears();
      this.CopyEnchantments((Tool) this, (Tool) destination);
      destination._GetOneFrom((Item) this);
      return (Item) destination;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14240");

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14241");

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
      switch (location)
      {
        case Farm _:
          this.animal = Utility.GetBestHarvestableFarmAnimal((IEnumerable<FarmAnimal>) (location as Farm).animals.Values, (Tool) this, toolRect);
          break;
        case AnimalHouse _:
          this.animal = Utility.GetBestHarvestableFarmAnimal((IEnumerable<FarmAnimal>) (location as AnimalHouse).animals.Values, (Tool) this, toolRect);
          break;
      }
      who.Halt();
      int currentFrame = who.FarmerSprite.CurrentFrame;
      who.FarmerSprite.animateOnce(283 + who.FacingDirection, 50f, 4);
      who.FarmerSprite.oldFrame = currentFrame;
      who.UsingTool = true;
      who.CanMove = false;
      return true;
    }

    public static void playSnip(Farmer who) => who.currentLocation.playSound("scissors");

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
      Shears.playSnip(who);
      this.CurrentParentTileIndex = 7;
      this.IndexOfMenuItemView = 7;
      if (this.animal != null && (int) (NetFieldBase<int, NetInt>) this.animal.currentProduce > 0 && (int) (NetFieldBase<int, NetInt>) this.animal.age >= (int) (byte) (NetFieldBase<byte, NetByte>) this.animal.ageWhenMature && this.animal.toolUsedForHarvest.Equals((object) this.BaseName))
      {
        if (who.addItemToInventoryBool((Item) new StardewValley.Object(Vector2.Zero, (int) (NetFieldBase<int, NetInt>) this.animal.currentProduce, (string) null, false, true, false, false)
        {
          Quality = (int) (NetFieldBase<int, NetInt>) this.animal.produceQuality
        }))
        {
          Utility.RecordAnimalProduce(this.animal, (int) (NetFieldBase<int, NetInt>) this.animal.currentProduce);
          this.animal.currentProduce.Value = -1;
          Game1.playSound("coin");
          this.animal.friendshipTowardFarmer.Value = Math.Min(1000, (int) (NetFieldBase<int, NetInt>) this.animal.friendshipTowardFarmer + 5);
          if ((bool) (NetFieldBase<bool, NetBool>) this.animal.showDifferentTextureWhenReadyForHarvest)
            this.animal.Sprite.LoadTexture("Animals\\Sheared" + this.animal.type.Value);
          who.gainExperience(0, 5);
        }
      }
      else
      {
        string dialogue = "";
        if (this.animal != null && !this.animal.toolUsedForHarvest.Equals((object) this.BaseName))
          dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14245", (object) this.animal.displayName);
        if (this.animal != null && this.animal.isBaby() && this.animal.toolUsedForHarvest.Equals((object) this.BaseName))
          dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14246", (object) this.animal.displayName);
        if (this.animal != null && (int) (NetFieldBase<int, NetInt>) this.animal.age >= (int) (byte) (NetFieldBase<byte, NetByte>) this.animal.ageWhenMature && this.animal.toolUsedForHarvest.Equals((object) this.BaseName))
          dialogue = Game1.content.LoadString("Strings\\StringsFromCSFiles:Shears.cs.14247", (object) this.animal.displayName);
        if (dialogue.Length > 0)
          Game1.drawObjectDialogue(dialogue);
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
