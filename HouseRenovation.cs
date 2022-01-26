// Decompiled with JetBrains decompiler
// Type: StardewValley.HouseRenovation
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.GameData.HomeRenovations;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace StardewValley
{
  public class HouseRenovation : ISalable
  {
    protected string _displayName;
    protected string _name;
    protected string _description;
    public HouseRenovation.AnimationType animationType;
    public List<List<Rectangle>> renovationBounds = new List<List<Rectangle>>();
    public string placementText = "";
    public GameLocation location;
    public bool requireClearance = true;
    public Action<HouseRenovation, int> onRenovation;
    public Func<HouseRenovation, int, bool> validate;

    public bool ShouldDrawIcon() => false;

    public string DisplayName => this._displayName;

    public void drawInMenu(
      SpriteBatch spriteBatch,
      Vector2 location,
      float scaleSize,
      float transparency,
      float layerDepth,
      StackDrawType drawStackNumber,
      Color color,
      bool drawShadow)
    {
    }

    public string Name => this._name;

    public string getDescription() => this._description;

    public int maximumStackSize() => 1;

    public int addToStack(Item stack) => 0;

    public int Stack
    {
      get => 1;
      set
      {
      }
    }

    public int salePrice() => 0;

    public bool actionWhenPurchased() => false;

    public bool canStackWith(ISalable other) => false;

    public bool CanBuyItem(Farmer farmer) => true;

    public bool IsInfiniteStock() => true;

    public ISalable GetSalableInstance() => (ISalable) this;

    public static void ShowRenovationMenu() => Game1.activeClickableMenu = (IClickableMenu) new ShopMenu(HouseRenovation.GetAvailableRenovations(), on_purchase: new Func<ISalable, Farmer, int, bool>(HouseRenovation.OnPurchaseRenovation));

    public static List<ISalable> GetAvailableRenovations()
    {
      FarmHouse farmhouse = Game1.getLocationFromName((string) (NetFieldBase<string, NetString>) Game1.player.homeLocation) as FarmHouse;
      List<ISalable> availableRenovations = new List<ISalable>();
      Dictionary<string, HomeRenovation> dictionary = Game1.content.Load<Dictionary<string, HomeRenovation>>("Data\\HomeRenovations");
      foreach (string key in dictionary.Keys)
      {
        HomeRenovation homeRenovation = dictionary[key];
        bool flag1 = true;
        foreach (RenovationValue requirement in homeRenovation.Requirements)
        {
          if (requirement.Type == "Value")
          {
            string s = requirement.Value;
            bool flag2 = true;
            if (s.Length > 0 && s[0] == '!')
            {
              s = s.Substring(1);
              flag2 = false;
            }
            int num = int.Parse(s);
            try
            {
              NetInt netInt = (NetInt) farmhouse.GetType().GetField(requirement.Key).GetValue((object) farmhouse);
              if ((NetFieldBase<int, NetInt>) netInt == (NetInt) null)
              {
                flag1 = false;
                break;
              }
              if (netInt.Value == num != flag2)
              {
                flag1 = false;
                break;
              }
            }
            catch (Exception ex)
            {
              flag1 = false;
              break;
            }
          }
          else if (requirement.Type == "Mail")
          {
            string str = requirement.Value;
            if (Game1.player.hasOrWillReceiveMail(requirement.Key) != (requirement.Value == "1"))
            {
              flag1 = false;
              break;
            }
          }
        }
        if (flag1)
        {
          HouseRenovation houseRenovation = new HouseRenovation();
          houseRenovation.location = (GameLocation) farmhouse;
          houseRenovation._name = key;
          string[] strArray = Game1.content.LoadString(homeRenovation.TextStrings).Split('/');
          try
          {
            houseRenovation._displayName = strArray[0];
            houseRenovation._description = strArray[1];
            houseRenovation.placementText = strArray[2];
          }
          catch (Exception ex)
          {
            houseRenovation._displayName = "?";
            houseRenovation._description = "?";
            houseRenovation.placementText = "?";
          }
          if (homeRenovation.CheckForObstructions)
            houseRenovation.validate += new Func<HouseRenovation, int, bool>(HouseRenovation.EnsureNoObstructions);
          houseRenovation.animationType = !(homeRenovation.AnimationType == "destroy") ? HouseRenovation.AnimationType.Build : HouseRenovation.AnimationType.Destroy;
          if (homeRenovation.SpecialRect != null && homeRenovation.SpecialRect != "")
          {
            if (homeRenovation.SpecialRect == "crib")
            {
              Rectangle? cribBounds = farmhouse.GetCribBounds();
              if (farmhouse.CanModifyCrib() && cribBounds.HasValue)
                houseRenovation.AddRenovationBound(cribBounds.Value);
              else
                continue;
            }
          }
          else
          {
            foreach (RectGroup rectGroup in homeRenovation.RectGroups)
            {
              List<Rectangle> bounds = new List<Rectangle>();
              foreach (Rect rect in rectGroup.Rects)
                bounds.Add(new Rectangle()
                {
                  X = rect.X,
                  Y = rect.Y,
                  Width = rect.Width,
                  Height = rect.Height
                });
              houseRenovation.AddRenovationBound(bounds);
            }
          }
          foreach (RenovationValue renovateAction in homeRenovation.RenovateActions)
          {
            RenovationValue action_data = renovateAction;
            if (action_data.Type == "Value")
            {
              try
              {
                NetInt field = (NetInt) farmhouse.GetType().GetField(action_data.Key).GetValue((object) farmhouse);
                if ((NetFieldBase<int, NetInt>) field == (NetInt) null)
                {
                  flag1 = false;
                  break;
                }
                Action<HouseRenovation, int> action = (Action<HouseRenovation, int>) ((selected_renovation, index) =>
                {
                  if (action_data.Value == "selected")
                    field.Value = index;
                  else
                    field.Value = int.Parse(action_data.Value);
                });
                houseRenovation.onRenovation += action;
              }
              catch (Exception ex)
              {
                flag1 = false;
                break;
              }
            }
            else if (action_data.Type == "Mail")
            {
              Action<HouseRenovation, int> action = (Action<HouseRenovation, int>) ((selected_renovation, index) =>
              {
                if (action_data.Value == "0")
                  Game1.player.mailReceived.Remove(action_data.Key);
                else
                  Game1.player.mailReceived.Add(action_data.Key);
              });
              houseRenovation.onRenovation += action;
            }
          }
          if (flag1)
          {
            houseRenovation.onRenovation += (Action<HouseRenovation, int>) ((a, b) => farmhouse.UpdateForRenovation());
            availableRenovations.Add((ISalable) houseRenovation);
          }
        }
      }
      return availableRenovations;
    }

    public static bool EnsureNoObstructions(HouseRenovation renovation, int selected_index)
    {
      if (renovation.location == null)
        return false;
      foreach (Rectangle rectangle1 in renovation.renovationBounds[selected_index])
      {
        for (int left = rectangle1.Left; left < rectangle1.Right; ++left)
        {
          for (int top = rectangle1.Top; top < rectangle1.Bottom; ++top)
          {
            if (renovation.location.isTileOccupiedByFarmer(new Vector2((float) left, (float) top)) != null)
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:RenovationBlocked"));
              return false;
            }
            if (renovation.location.isTileOccupied(new Vector2((float) left, (float) top)))
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:RenovationBlocked"));
              return false;
            }
          }
        }
        Rectangle rectangle2 = new Rectangle(rectangle1.X * 64, rectangle1.Y * 64, rectangle1.Width * 64, rectangle1.Height * 64);
        if (renovation.location is DecoratableLocation location)
        {
          foreach (Furniture furniture in location.furniture)
          {
            if (furniture.getBoundingBox((Vector2) (NetFieldBase<Vector2, NetVector2>) furniture.tileLocation).Intersects(rectangle2))
            {
              Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:RenovationBlocked"));
              return false;
            }
          }
        }
      }
      return true;
    }

    public static void BuildCrib(HouseRenovation renovation, int selected_index)
    {
      if (!(renovation.location is FarmHouse location))
        return;
      location.cribStyle.Value = 1;
    }

    public static void RemoveCrib(HouseRenovation renovation, int selected_index)
    {
      if (!(renovation.location is FarmHouse location))
        return;
      location.cribStyle.Value = 0;
    }

    public static void OpenBedroom(HouseRenovation renovation, int selected_index)
    {
      if (!(renovation.location is FarmHouse location))
        return;
      Game1.player.mailReceived.Add("renovation_bedroom_open");
      location.UpdateForRenovation();
    }

    public static void CloseBedroom(HouseRenovation renovation, int selected_index)
    {
      if (!(renovation.location is FarmHouse location))
        return;
      Game1.player.mailReceived.Remove("renovation_bedroom_open");
      location.UpdateForRenovation();
    }

    public static void OpenSouthernRoom(HouseRenovation renovation, int selected_index)
    {
      if (!(renovation.location is FarmHouse location))
        return;
      Game1.player.mailReceived.Add("renovation_southern_open");
      location.UpdateForRenovation();
    }

    public static void CloseSouthernRoom(HouseRenovation renovation, int selected_index)
    {
      if (!(renovation.location is FarmHouse location))
        return;
      Game1.player.mailReceived.Remove("renovation_southern_open");
      location.UpdateForRenovation();
    }

    public static void OpenCornernRoom(HouseRenovation renovation, int selected_index)
    {
      if (!(renovation.location is FarmHouse location))
        return;
      Game1.player.mailReceived.Add("renovation_corner_open");
      location.UpdateForRenovation();
    }

    public static void CloseCornerRoom(HouseRenovation renovation, int selected_index)
    {
      if (!(renovation.location is FarmHouse location))
        return;
      Game1.player.mailReceived.Remove("renovation_corner_open");
      location.UpdateForRenovation();
    }

    public static bool OnPurchaseRenovation(ISalable salable, Farmer who, int amount)
    {
      if (!(salable is HouseRenovation renovation))
        return false;
      Game1.activeClickableMenu = (IClickableMenu) new RenovateMenu(renovation);
      return true;
    }

    public virtual void AddRenovationBound(Rectangle bound) => this.renovationBounds.Add(new List<Rectangle>()
    {
      bound
    });

    public virtual void AddRenovationBound(List<Rectangle> bounds) => this.renovationBounds.Add(bounds);

    public enum AnimationType
    {
      Build,
      Destroy,
    }
  }
}
