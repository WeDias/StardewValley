// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.GenericTool
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley.Tools
{
  /// <summary>
  /// Used as placeholder tools in certain situations. (e.g. as a "spot" in Clint's shop for the trash can upgrade)
  /// </summary>
  public class GenericTool : Tool
  {
    public string description;

    public GenericTool()
    {
    }

    public GenericTool(
      string name,
      string description,
      int upgradeLevel,
      int parentSheetIndex,
      int menuViewIndex)
      : base(name, upgradeLevel, parentSheetIndex, menuViewIndex, false)
    {
      this.description = description;
    }

    public override Item getOne()
    {
      GenericTool one = new GenericTool(this.BaseName, this.description, this.UpgradeLevel, this.InitialParentTileIndex, this.IndexOfMenuItemView);
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    protected override string loadDescription() => this.description;

    protected override string loadDisplayName() => this.BaseName;
  }
}
