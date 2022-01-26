// Decompiled with JetBrains decompiler
// Type: StardewValley.Events.SoundInTheNightEvent
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Buildings;
using StardewValley.Network;
using StardewValley.TerrainFeatures;
using System;

namespace StardewValley.Events
{
  public class SoundInTheNightEvent : FarmEvent, INetObject<NetFields>
  {
    public const int cropCircle = 0;
    public const int meteorite = 1;
    public const int dogs = 2;
    public const int owl = 3;
    public const int earthquake = 4;
    private readonly NetInt behavior = new NetInt();
    private int timer;
    private string soundName;
    private string message;
    private bool playedSound;
    private bool showedMessage;
    private Vector2 targetLocation;
    private Building targetBuilding;

    public NetFields NetFields { get; } = new NetFields();

    public SoundInTheNightEvent() => this.NetFields.AddField((INetSerializable) this.behavior);

    public SoundInTheNightEvent(int which)
      : this()
    {
      this.behavior.Value = which;
    }

    public bool setUp()
    {
      Random random = new Random((int) Game1.uniqueIDForThisGame + (int) Game1.stats.DaysPlayed);
      Farm locationFromName = Game1.getLocationFromName("Farm") as Farm;
      locationFromName.updateMap();
      switch ((int) (NetFieldBase<int, NetInt>) this.behavior)
      {
        case 0:
          this.soundName = "UFO";
          this.message = Game1.content.LoadString("Strings\\Events:SoundInTheNight_UFO");
          int num1;
          for (num1 = 50; num1 > 0; --num1)
          {
            this.targetLocation = new Vector2((float) random.Next(5, locationFromName.map.GetLayer("Back").TileWidth - 4), (float) random.Next(5, locationFromName.map.GetLayer("Back").TileHeight - 4));
            if (locationFromName.isTileLocationTotallyClearAndPlaceable(this.targetLocation))
              break;
          }
          if (num1 <= 0)
            return true;
          break;
        case 1:
          this.soundName = "Meteorite";
          this.message = Game1.content.LoadString("Strings\\Events:SoundInTheNight_Meteorite");
          this.targetLocation = new Vector2((float) random.Next(5, locationFromName.map.GetLayer("Back").TileWidth - 20), (float) random.Next(5, locationFromName.map.GetLayer("Back").TileHeight - 4));
          for (int x = (int) this.targetLocation.X; (double) x <= (double) this.targetLocation.X + 1.0; ++x)
          {
            for (int y = (int) this.targetLocation.Y; (double) y <= (double) this.targetLocation.Y + 1.0; ++y)
            {
              Vector2 tile = new Vector2((float) x, (float) y);
              if (!locationFromName.isTileOpenBesidesTerrainFeatures(tile) || !locationFromName.isTileOpenBesidesTerrainFeatures(new Vector2(tile.X + 1f, tile.Y)) || !locationFromName.isTileOpenBesidesTerrainFeatures(new Vector2(tile.X + 1f, tile.Y - 1f)) || !locationFromName.isTileOpenBesidesTerrainFeatures(new Vector2(tile.X, tile.Y - 1f)) || locationFromName.doesTileHaveProperty((int) tile.X, (int) tile.Y, "Water", "Back") != null || locationFromName.doesTileHaveProperty((int) tile.X + 1, (int) tile.Y, "Water", "Back") != null)
                return true;
            }
          }
          break;
        case 2:
          this.soundName = "dogs";
          if (random.NextDouble() < 0.5)
            return true;
          foreach (Building building in locationFromName.buildings)
          {
            if (building.indoors.Value != null && building.indoors.Value is AnimalHouse && !(bool) (NetFieldBase<bool, NetBool>) building.animalDoorOpen && (building.indoors.Value as AnimalHouse).animalsThatLiveHere.Count > (building.indoors.Value as AnimalHouse).animals.Count() && random.NextDouble() < 1.0 / (double) locationFromName.buildings.Count)
            {
              this.targetBuilding = building;
              break;
            }
          }
          return this.targetBuilding == null;
        case 3:
          this.soundName = "owl";
          int num2;
          for (num2 = 50; num2 > 0; --num2)
          {
            this.targetLocation = new Vector2((float) random.Next(5, locationFromName.map.GetLayer("Back").TileWidth - 4), (float) random.Next(5, locationFromName.map.GetLayer("Back").TileHeight - 4));
            if (locationFromName.isTileLocationTotallyClearAndPlaceable(this.targetLocation))
              break;
          }
          if (num2 <= 0)
            return true;
          break;
        case 4:
          this.soundName = "thunder_small";
          this.message = Game1.content.LoadString("Strings\\Events:SoundInTheNight_Earthquake");
          break;
      }
      Game1.freezeControls = true;
      return false;
    }

    public bool tickUpdate(GameTime time)
    {
      this.timer += time.ElapsedGameTime.Milliseconds;
      if (this.timer > 1500 && !this.playedSound)
      {
        if (this.soundName != null && !this.soundName.Equals(""))
        {
          Game1.playSound(this.soundName);
          this.playedSound = true;
        }
        if (!this.playedSound && this.message != null)
        {
          Game1.drawObjectDialogue(this.message);
          Game1.globalFadeToClear();
          this.showedMessage = true;
        }
      }
      if (this.timer > 7000 && !this.showedMessage)
      {
        Game1.pauseThenMessage(10, this.message, false);
        this.showedMessage = true;
      }
      if (!this.showedMessage || !this.playedSound)
        return false;
      Game1.freezeControls = false;
      return true;
    }

    public void draw(SpriteBatch b)
    {
      SpriteBatch spriteBatch = b;
      Texture2D staminaRect = Game1.staminaRect;
      Viewport viewport = Game1.graphics.GraphicsDevice.Viewport;
      int width = viewport.Width;
      viewport = Game1.graphics.GraphicsDevice.Viewport;
      int height = viewport.Height;
      Rectangle destinationRectangle = new Rectangle(0, 0, width, height);
      Color black = Color.Black;
      spriteBatch.Draw(staminaRect, destinationRectangle, black);
    }

    public void makeChangesToLocation()
    {
      if (!Game1.IsMasterGame)
        return;
      Farm locationFromName = Game1.getLocationFromName("Farm") as Farm;
      switch ((int) (NetFieldBase<int, NetInt>) this.behavior)
      {
        case 0:
          StardewValley.Object @object = new StardewValley.Object(this.targetLocation, 96);
          @object.minutesUntilReady.Value = 24000 - Game1.timeOfDay;
          locationFromName.objects.Add(this.targetLocation, @object);
          break;
        case 1:
          if (locationFromName.terrainFeatures.ContainsKey(this.targetLocation))
            locationFromName.terrainFeatures.Remove(this.targetLocation);
          if (locationFromName.terrainFeatures.ContainsKey(this.targetLocation + new Vector2(1f, 0.0f)))
            locationFromName.terrainFeatures.Remove(this.targetLocation + new Vector2(1f, 0.0f));
          if (locationFromName.terrainFeatures.ContainsKey(this.targetLocation + new Vector2(1f, 1f)))
            locationFromName.terrainFeatures.Remove(this.targetLocation + new Vector2(1f, 1f));
          if (locationFromName.terrainFeatures.ContainsKey(this.targetLocation + new Vector2(0.0f, 1f)))
            locationFromName.terrainFeatures.Remove(this.targetLocation + new Vector2(0.0f, 1f));
          locationFromName.resourceClumps.Add(new ResourceClump(622, 2, 2, this.targetLocation));
          break;
        case 2:
          AnimalHouse animalHouse = this.targetBuilding.indoors.Value as AnimalHouse;
          long key1 = 0;
          foreach (long key2 in (NetList<long, NetLong>) animalHouse.animalsThatLiveHere)
          {
            if (!animalHouse.animals.ContainsKey(key2))
            {
              key1 = key2;
              break;
            }
          }
          if (!Game1.getFarm().animals.ContainsKey(key1))
            break;
          Game1.getFarm().animals.Remove(key1);
          animalHouse.animalsThatLiveHere.Remove(key1);
          using (NetDictionary<long, FarmAnimal, NetRef<FarmAnimal>, SerializableDictionary<long, FarmAnimal>, NetLongDictionary<FarmAnimal, NetRef<FarmAnimal>>>.PairsCollection.Enumerator enumerator = Game1.getFarm().animals.Pairs.GetEnumerator())
          {
            while (enumerator.MoveNext())
              enumerator.Current.Value.moodMessage.Value = 5;
            break;
          }
        case 3:
          locationFromName.objects.Add(this.targetLocation, new StardewValley.Object(this.targetLocation, 95));
          break;
      }
    }

    public void drawAboveEverything(SpriteBatch b)
    {
    }
  }
}
