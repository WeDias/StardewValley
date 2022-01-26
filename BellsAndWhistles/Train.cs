// Decompiled with JetBrains decompiler
// Type: StardewValley.BellsAndWhistles.Train
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley.Monsters;
using StardewValley.Network;
using System;

namespace StardewValley.BellsAndWhistles
{
  public class Train : INetObject<NetFields>
  {
    public const int minCars = 8;
    public const int maxCars = 24;
    public const double chanceForLongTrain = 0.1;
    public const int randomTrain = 0;
    public const int jojaTrain = 1;
    public const int coalTrain = 2;
    public const int passengerTrain = 3;
    public const int uniformColorPlainTrain = 4;
    public const int prisonTrain = 5;
    public const int christmasTrain = 6;
    public readonly NetObjectList<TrainCar> cars = new NetObjectList<TrainCar>();
    public readonly NetInt type = new NetInt();
    public readonly NetPosition position = new NetPosition();
    public float speed;
    public float wheelRotation;
    public float smokeTimer;
    private TemporaryAnimatedSprite whistleSteam;

    public NetFields NetFields { get; } = new NetFields();

    public Train()
    {
      this.initNetFields();
      Random random = new Random();
      if (random.NextDouble() < 0.1)
        this.type.Value = 3;
      else if (random.NextDouble() < 0.1)
        this.type.Value = 1;
      else if (random.NextDouble() < 0.1)
        this.type.Value = 2;
      else if (random.NextDouble() < 0.05)
        this.type.Value = 5;
      else if (Game1.currentSeason.ToLower().Equals("winter") && random.NextDouble() < 0.2)
        this.type.Value = 6;
      else
        this.type.Value = 0;
      int num1 = random.Next(8, 25);
      if (random.NextDouble() < 0.1)
        num1 *= 2;
      this.speed = 0.2f;
      this.smokeTimer = this.speed * 2000f;
      Color color1 = Color.White;
      double num2 = 1.0;
      double num3 = 1.0;
      switch ((int) (NetFieldBase<int, NetInt>) this.type)
      {
        case 0:
          num2 = 0.2;
          num3 = 0.2;
          break;
        case 1:
          num2 = 0.0;
          num3 = 0.0;
          color1 = Color.DimGray;
          break;
        case 2:
          num2 = 0.0;
          num3 = 0.7;
          break;
        case 3:
          num2 = 1.0;
          num3 = 0.0;
          this.speed = 0.4f;
          break;
        case 5:
          num3 = 0.0;
          num2 = 0.0;
          color1 = Color.MediumBlue;
          this.speed = 0.4f;
          break;
        case 6:
          num2 = 0.0;
          num3 = 1.0;
          color1 = Color.Red;
          break;
      }
      this.cars.Add(new TrainCar(random, 3, -1, Color.White));
      for (int index = 1; index < num1; ++index)
      {
        int carType = 0;
        if (random.NextDouble() < num2)
          carType = 2;
        else if (random.NextDouble() < num3)
          carType = 1;
        Color color2 = color1;
        if (color1.Equals(Color.White))
        {
          bool flag1 = false;
          bool flag2 = false;
          bool flag3 = false;
          switch (random.Next(3))
          {
            case 0:
              flag1 = true;
              break;
            case 1:
              flag2 = true;
              break;
            case 2:
              flag3 = true;
              break;
          }
          color2 = new Color(random.Next(flag1 ? 0 : 100, 250), random.Next(flag2 ? 0 : 100, 250), random.Next(flag3 ? 0 : 100, 250));
        }
        int frontDecal = -1;
        if ((int) (NetFieldBase<int, NetInt>) this.type == 1)
          frontDecal = 2;
        else if ((int) (NetFieldBase<int, NetInt>) this.type == 5)
          frontDecal = 1;
        else if ((int) (NetFieldBase<int, NetInt>) this.type == 6)
          frontDecal = -1;
        else if (random.NextDouble() < 0.3)
          frontDecal = random.Next(35);
        int resourceType = 0;
        if (carType == 1)
        {
          resourceType = random.Next(9);
          if ((int) (NetFieldBase<int, NetInt>) this.type == 6)
            resourceType = 9;
        }
        this.cars.Add(new TrainCar(random, carType, frontDecal, color2, resourceType, random.Next(4, 10)));
      }
    }

    private void initNetFields() => this.NetFields.AddFields((INetSerializable) this.cars, (INetSerializable) this.type, (INetSerializable) this.position.NetFields);

    private void Position_fieldChangeEvent(NetFloat field, float oldValue, float newValue) => Console.WriteLine("ChangeedD: " + newValue.ToString());

    private void Position_fieldChangeVisibleEvent(NetFloat field, float oldValue, float newValue) => Console.WriteLine("newVal: " + newValue.ToString());

    public Rectangle getBoundingBox() => new Rectangle(-this.cars.Count * 128 * 4 + (int) this.position.X, 2720, this.cars.Count * 128 * 4, 128);

    public bool Update(GameTime time, GameLocation location)
    {
      if (Game1.IsMasterGame)
        this.position.X += (float) time.ElapsedGameTime.Milliseconds * this.speed;
      this.wheelRotation += (float) time.ElapsedGameTime.Milliseconds * ((float) Math.PI / 256f);
      this.wheelRotation %= 6.283185f;
      if (!Game1.eventUp && location.Equals(Game1.currentLocation))
      {
        Farmer player = Game1.player;
        Rectangle boundingBox = player.GetBoundingBox();
        if (boundingBox.Intersects(this.getBoundingBox()))
        {
          player.xVelocity = 8f;
          Farmer farmer = player;
          boundingBox = this.getBoundingBox();
          int y1 = boundingBox.Center.Y;
          boundingBox = player.GetBoundingBox();
          int y2 = boundingBox.Center.Y;
          double num = (double) (y1 - y2) / 4.0;
          farmer.yVelocity = (float) num;
          player.takeDamage(20, true, (Monster) null);
          if (player.UsingTool)
            Game1.playSound("clank");
        }
      }
      if (Game1.random.NextDouble() < 0.001 && location.Equals(Game1.currentLocation))
      {
        Game1.playSound("trainWhistle");
        this.whistleSteam = new TemporaryAnimatedSprite(27, new Vector2(this.position.X - 250f, 2624f), Color.White, sourceRectWidth: 64, layerDepth: 1f, sourceRectHeight: 64);
      }
      if (this.whistleSteam != null)
      {
        this.whistleSteam.Position = new Vector2(this.position.X - 258f, 2592f);
        if (this.whistleSteam.update(time))
          this.whistleSteam = (TemporaryAnimatedSprite) null;
      }
      this.smokeTimer -= (float) time.ElapsedGameTime.Milliseconds;
      if ((double) this.smokeTimer <= 0.0)
      {
        location.temporarySprites.Add(new TemporaryAnimatedSprite(25, new Vector2(this.position.X - 170f, 2496f), Color.White, sourceRectWidth: 64, layerDepth: 1f, sourceRectHeight: 128));
        this.smokeTimer = this.speed * 2000f;
      }
      return (double) this.position.X > (double) (this.cars.Count * 128 * 4 + 4480);
    }

    public void draw(SpriteBatch b)
    {
      for (int index = 0; index < this.cars.Count; ++index)
        this.cars[index].draw(b, new Vector2(this.position.X - (float) ((index + 1) * 512), 2592f), this.wheelRotation);
      if (this.whistleSteam == null)
        return;
      this.whistleSteam.draw(b);
    }
  }
}
