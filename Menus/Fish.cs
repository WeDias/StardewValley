// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.Fish
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  public class Fish
  {
    public const int widthOfTrack = 1020;
    public const int msPerFrame = 65;
    public const int fishingFieldWidth = 1028;
    public const int fishingFieldHeight = 612;
    public int whichFish;
    public int indexOfAnimation;
    public int animationTimer = 65;
    public float chanceToDart;
    public float dartingRandomness;
    public float dartingIntensity;
    public float dartingDuration;
    public float dartingTimer;
    public float dartingExtraSpeed;
    public float turnFrequency;
    public float turnSpeed;
    public float turnIntensity;
    public float minSpeed;
    public float maxSpeed;
    public float speedChangeFrequency;
    public float currentSpeed;
    public float targetSpeed;
    public float positionOnTrack = 510f;
    public Vector2 position;
    public float rotation;
    public float targetRotation;
    public bool isDarting;
    public Rectangle fishingField;
    private string fishName;
    public int bobberDifficulty;

    public Fish(int whichFish)
    {
      this.whichFish = whichFish;
      this.fishingField = new Rectangle(0, 0, 1028, 612);
      Dictionary<int, string> dictionary = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
      if (dictionary.ContainsKey(whichFish))
      {
        string[] strArray = dictionary[whichFish].Split('/');
        this.fishName = strArray[0];
        this.chanceToDart = (float) Convert.ToInt32(strArray[1]);
        this.dartingRandomness = (float) Convert.ToInt32(strArray[2]);
        this.dartingIntensity = (float) Convert.ToInt32(strArray[3]);
        this.dartingDuration = (float) Convert.ToInt32(strArray[4]);
        this.turnFrequency = (float) Convert.ToInt32(strArray[5]);
        this.turnSpeed = (float) Convert.ToInt32(strArray[6]);
        this.turnIntensity = (float) Convert.ToInt32(strArray[7]);
        this.minSpeed = (float) Convert.ToInt32(strArray[8]);
        this.maxSpeed = (float) Convert.ToInt32(strArray[9]);
        this.speedChangeFrequency = (float) Convert.ToInt32(strArray[10]);
        this.bobberDifficulty = Convert.ToInt32(strArray[11]);
      }
      this.position = new Vector2(514f, 306f);
      this.targetSpeed = this.minSpeed / 50f;
    }

    public bool isWithinRectangle(
      Rectangle r,
      int xPositionOfFishingField,
      int yPositionOfFishingField)
    {
      return r.Contains((int) this.position.X + xPositionOfFishingField, (int) this.position.Y + yPositionOfFishingField);
    }

    public void Update(GameTime time)
    {
      this.animationTimer -= time.ElapsedGameTime.Milliseconds;
      if (this.animationTimer <= 0)
      {
        this.animationTimer = 65 - (int) ((double) this.currentSpeed * 10.0);
        this.indexOfAnimation = (this.indexOfAnimation + 1) % 8;
      }
      if (!this.isDarting && Game1.random.NextDouble() < (double) this.chanceToDart / 10000.0)
      {
        this.rotation += (float) ((double) Game1.random.Next(-(int) this.dartingRandomness, (int) this.dartingRandomness) * Math.PI / 100.0);
        this.targetSpeed = this.rotation;
        this.dartingExtraSpeed = this.dartingIntensity / 20f;
        this.dartingExtraSpeed *= (float) (1.0 + (double) Game1.random.Next(-10, 10) / 100.0);
        this.dartingTimer = (float) ((double) this.dartingDuration * 10.0 + (double) Game1.random.Next(-(int) this.dartingDuration, (int) this.dartingDuration) * 0.100000001490116);
        this.isDarting = true;
      }
      if ((double) this.dartingTimer > 0.0)
      {
        this.dartingTimer -= (float) time.ElapsedGameTime.Milliseconds;
        if ((double) this.dartingTimer <= 0.0 && this.isDarting)
        {
          this.isDarting = false;
          this.dartingTimer = (float) ((double) this.dartingDuration * 10.0 + (double) Game1.random.Next(-(int) this.dartingDuration, (int) this.dartingDuration) * 0.100000001490116);
        }
        if (!this.isDarting)
          this.dartingExtraSpeed -= this.dartingExtraSpeed * 0.0005f * (float) time.ElapsedGameTime.Milliseconds;
      }
      if (Game1.random.NextDouble() < (double) this.turnFrequency / 10000.0)
        this.targetRotation = (float) ((double) Game1.random.Next((int) -(double) this.turnIntensity, (int) this.turnIntensity) / 100.0 * Math.PI);
      if (Game1.random.NextDouble() < (double) this.speedChangeFrequency / 10000.0)
        this.targetSpeed = (float) (int) ((double) Game1.random.Next((int) this.minSpeed, (int) this.maxSpeed) / 20.0);
      if ((double) Math.Abs(this.rotation - this.targetRotation) > (double) Math.Abs(this.targetRotation / (100f - this.turnSpeed)))
        this.rotation += this.targetRotation / (100f - this.turnSpeed);
      this.rotation %= 6.283185f;
      this.currentSpeed += (float) (((double) this.targetSpeed - (double) this.currentSpeed) / 10.0);
      this.currentSpeed = Math.Min(this.maxSpeed / 20f, this.currentSpeed);
      this.currentSpeed = Math.Max(this.minSpeed / 20f, this.currentSpeed);
      this.position.X += this.currentSpeed * (float) Math.Cos((double) this.rotation);
      int num = 0;
      if (!this.fishingField.Contains(new Rectangle((int) this.position.X - 32, (int) this.position.Y - 32, 64, 64)))
      {
        Vector2 vector2 = new Vector2(this.currentSpeed * (float) Math.Cos((double) this.rotation), this.currentSpeed * (float) Math.Sin((double) this.rotation));
        vector2.X = -vector2.X;
        this.rotation = (float) Math.Atan((double) vector2.Y / (double) vector2.X);
        if ((double) vector2.X < 0.0)
          this.rotation += 3.141593f;
        else if ((double) vector2.Y < 0.0)
          this.rotation += 1.570796f;
        this.position.X += this.currentSpeed * (float) Math.Cos((double) this.rotation);
        ++num;
      }
      this.position.Y += this.currentSpeed * (float) Math.Sin((double) this.rotation);
      if (!this.fishingField.Contains(new Rectangle((int) this.position.X - 32, (int) this.position.Y - 32, 64, 64)))
      {
        Vector2 vector2 = new Vector2(this.currentSpeed * (float) Math.Cos((double) this.rotation), this.currentSpeed * (float) Math.Sin((double) this.rotation));
        vector2.Y = -vector2.Y;
        this.rotation = (float) Math.Atan((double) vector2.Y / (double) vector2.X);
        if ((double) vector2.X < 0.0)
          this.rotation += 3.141593f;
        else if ((double) vector2.Y > 0.0)
          this.rotation += 1.570796f;
        this.position.Y += this.currentSpeed * (float) Math.Sin((double) this.rotation);
        ++num;
      }
      if (num >= 2)
      {
        Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(new Point((int) this.position.X, (int) this.position.Y), new Vector2(514f, 306f), this.currentSpeed);
        this.rotation = (float) Math.Atan((double) velocityTowardPoint.Y / (double) velocityTowardPoint.X);
        if ((double) velocityTowardPoint.X < 0.0)
          this.rotation += 3.141593f;
        else if ((double) velocityTowardPoint.Y < 0.0)
          this.rotation += 1.570796f;
        this.position.X += this.currentSpeed * (float) Math.Cos((double) this.rotation);
        this.position.Y += this.currentSpeed * (float) Math.Sin((double) this.rotation);
      }
      else
      {
        if (num != 1)
          return;
        this.targetRotation = this.rotation;
      }
    }

    public void draw(SpriteBatch b, Vector2 positionOfFishingField) => b.Draw(Game1.mouseCursors, this.position + positionOfFishingField, new Rectangle?(new Rectangle(561, 1846 + this.indexOfAnimation * 16, 16, 16)), Color.White, this.rotation + 1.570796f, new Vector2(8f, 8f), 4f, SpriteEffects.None, 0.5f);
  }
}
