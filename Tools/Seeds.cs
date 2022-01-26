// Decompiled with JetBrains decompiler
// Type: StardewValley.Tools.Seeds
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley.Tools
{
  public class Seeds : Stackable
  {
    private string seedType;
    private int numberInStack;

    public new int NumberInStack
    {
      get => this.numberInStack;
      set => this.numberInStack = value;
    }

    public string SeedType
    {
      get => this.seedType;
      set => this.seedType = value;
    }

    public Seeds()
    {
    }

    public Seeds(string seedType, int numberInStack)
      : base(nameof (Seeds), 0, 0, 0, true)
    {
      this.seedType = seedType;
      this.numberInStack = numberInStack;
      this.setCurrentTileIndexToSeedType();
      this.IndexOfMenuItemView = this.CurrentParentTileIndex;
    }

    public override Item getOne()
    {
      Seeds one = new Seeds(this.SeedType, 1);
      one._GetOneFrom((Item) this);
      return (Item) one;
    }

    protected override string loadDisplayName() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Seeds.cs.14209");

    protected override string loadDescription() => Game1.content.LoadString("Strings\\StringsFromCSFiles:Seeds.cs.14210");

    public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who)
    {
      who.Stamina -= (float) (2.0 - (double) who.FarmingLevel * 0.100000001490116);
      --this.numberInStack;
      this.setCurrentTileIndexToSeedType();
      location.playSound("seeds");
    }

    private void setCurrentTileIndexToSeedType()
    {
      string seedType = this.seedType;
      // ISSUE: reference to a compiler-generated method
      switch (\u003CPrivateImplementationDetails\u003E.ComputeStringHash(seedType))
      {
        case 100164663:
          if (!(seedType == "Rhubarb"))
            break;
          this.CurrentParentTileIndex = 6;
          break;
        case 121410417:
          if (!(seedType == "Beet"))
            break;
          this.CurrentParentTileIndex = 62;
          break;
        case 137760495:
          if (!(seedType == "Garlic"))
            break;
          this.CurrentParentTileIndex = 4;
          break;
        case 252262714:
          if (!(seedType == "Spring Mix"))
            break;
          this.CurrentParentTileIndex = 63;
          break;
        case 321905522:
          if (!(seedType == "Potato"))
            break;
          this.CurrentParentTileIndex = 3;
          break;
        case 795997688:
          if (!(seedType == "Radish"))
            break;
          this.CurrentParentTileIndex = 12;
          break;
        case 1020152658:
          if (!(seedType == "Summer Mix"))
            break;
          this.CurrentParentTileIndex = 64;
          break;
        case 1026502717:
          if (!(seedType == "Starfruit"))
            break;
          this.CurrentParentTileIndex = 14;
          break;
        case 1155325948:
          if (!(seedType == "Melon"))
            break;
          this.CurrentParentTileIndex = 7;
          break;
        case 1418370675:
          if (!(seedType == "Corn"))
            break;
          this.CurrentParentTileIndex = 15;
          break;
        case 1651195363:
          if (!(seedType == "Tomato"))
            break;
          this.CurrentParentTileIndex = 8;
          break;
        case 1787800187:
          if (!(seedType == "Eggplant"))
            break;
          this.CurrentParentTileIndex = 56;
          break;
        case 2051905485:
          if (!(seedType == "Cranberries"))
            break;
          this.CurrentParentTileIndex = 61;
          break;
        case 2309904358:
          if (!(seedType == "Kale"))
            break;
          this.CurrentParentTileIndex = 5;
          break;
        case 2401363451:
          if (!(seedType == "Yellow Pepper"))
            break;
          this.CurrentParentTileIndex = 10;
          break;
        case 2427154736:
          if (!(seedType == "Wheat"))
            break;
          this.CurrentParentTileIndex = 11;
          break;
        case 2510277530:
          if (!(seedType == "Winter Mix"))
            break;
          this.CurrentParentTileIndex = 66;
          break;
        case 2837304215:
          if (!(seedType == "Blueberry"))
            break;
          this.CurrentParentTileIndex = 9;
          break;
        case 2981019750:
          if (!(seedType == "Yam"))
            break;
          this.CurrentParentTileIndex = 60;
          break;
        case 3106606342:
          if (!(seedType == "Green Bean"))
            break;
          this.CurrentParentTileIndex = 1;
          break;
        case 3388885286:
          if (!(seedType == "Bok Choy"))
            break;
          this.CurrentParentTileIndex = 59;
          break;
        case 3468995526:
          if (!(seedType == "Fall Mix"))
            break;
          this.CurrentParentTileIndex = 65;
          break;
        case 3517106355:
          if (!(seedType == "Red Cabbage"))
            break;
          this.CurrentParentTileIndex = 13;
          break;
        case 3607993668:
          if (!(seedType == "Parsnip"))
            break;
          this.CurrentParentTileIndex = 0;
          break;
        case 3772079609:
          if (!(seedType == "Ancient Fruit"))
            break;
          this.CurrentParentTileIndex = 72;
          break;
        case 3782215428:
          if (!(seedType == "Cauliflower"))
            break;
          this.CurrentParentTileIndex = 2;
          break;
        case 3998561503:
          if (!(seedType == "Pumpkin"))
            break;
          this.CurrentParentTileIndex = 58;
          break;
        case 4181074131:
          if (!(seedType == "Artichoke"))
            break;
          this.CurrentParentTileIndex = 57;
          break;
      }
    }
  }
}
