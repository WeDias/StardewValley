// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.ToolFactory
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Netcode;

namespace StardewValley.Tools
{
  public class ToolFactory
  {
    public const byte axe = 0;
    public const byte hoe = 1;
    public const byte fishingRod = 2;
    public const byte pickAxe = 3;
    public const byte wateringCan = 4;
    public const byte meleeWeapon = 5;
    public const byte slingshot = 6;

    public static ToolDescription getIndexFromTool(Tool t)
    {
      switch (t)
      {
        case Axe _:
          return new ToolDescription((byte) 0, (byte) (int) (NetFieldBase<int, NetInt>) t.upgradeLevel);
        case Hoe _:
          return new ToolDescription((byte) 1, (byte) (int) (NetFieldBase<int, NetInt>) t.upgradeLevel);
        case FishingRod _:
          return new ToolDescription((byte) 2, (byte) (int) (NetFieldBase<int, NetInt>) t.upgradeLevel);
        case Pickaxe _:
          return new ToolDescription((byte) 3, (byte) (int) (NetFieldBase<int, NetInt>) t.upgradeLevel);
        case WateringCan _:
          return new ToolDescription((byte) 4, (byte) (int) (NetFieldBase<int, NetInt>) t.upgradeLevel);
        case MeleeWeapon _:
          return new ToolDescription((byte) 5, (byte) (int) (NetFieldBase<int, NetInt>) t.upgradeLevel);
        case Slingshot _:
          return new ToolDescription((byte) 6, (byte) (int) (NetFieldBase<int, NetInt>) t.upgradeLevel);
        default:
          return new ToolDescription((byte) 0, (byte) 0);
      }
    }

    public static Tool getToolFromDescription(byte index, int upgradeLevel)
    {
      Tool toolFromDescription = (Tool) null;
      switch (index)
      {
        case 0:
          toolFromDescription = (Tool) new Axe();
          break;
        case 1:
          toolFromDescription = (Tool) new Hoe();
          break;
        case 2:
          toolFromDescription = (Tool) new FishingRod();
          break;
        case 3:
          toolFromDescription = (Tool) new Pickaxe();
          break;
        case 4:
          toolFromDescription = (Tool) new WateringCan();
          break;
        case 5:
          toolFromDescription = (Tool) new MeleeWeapon(0, upgradeLevel);
          break;
        case 6:
          toolFromDescription = (Tool) new Slingshot();
          break;
      }
      toolFromDescription.UpgradeLevel = upgradeLevel;
      return toolFromDescription;
    }
  }
}
