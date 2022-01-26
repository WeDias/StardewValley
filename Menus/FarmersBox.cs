// Decompiled with JetBrains decompiler
// Type: StardewValley.Menus.FarmersBox
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using System.Collections.Generic;

namespace StardewValley.Menus
{
  internal class FarmersBox : IClickableMenu
  {
    private readonly List<Farmer> _farmers = new List<Farmer>();
    private readonly Texture2D _iconTex;
    public float _updateTimer;
    private readonly List<FarmerBoxButton> _profileButtons;
    private readonly List<FarmerBoxButton> _muteButtons;

    public FarmersBox()
      : base(0, 200, 528, 400)
    {
      this._muteButtons = new List<FarmerBoxButton>();
      this._profileButtons = new List<FarmerBoxButton>();
    }

    private void UpdateFarmers(List<ClickableComponent> parentComponents)
    {
      if ((double) this._updateTimer > 0.0)
        return;
      this._farmers.Clear();
      foreach (Farmer onlineFarmer in Game1.getOnlineFarmers())
        this._farmers.Add(onlineFarmer);
      this._updateTimer = 1f;
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
    }

    public override void update(GameTime time) => this._updateTimer -= (float) time.ElapsedGameTime.TotalSeconds;

    public void draw(
      SpriteBatch b,
      int left,
      int bottom,
      ClickableComponent current,
      List<ClickableComponent> parentComponents)
    {
      this.UpdateFarmers(parentComponents);
      if (this._farmers.Count == 0)
        return;
      int num = 100;
      this.height = num * this._farmers.Count;
      this.xPositionOnScreen = left;
      this.yPositionOnScreen = bottom - this.height;
      IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(301, 288, 15, 15), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, Color.White, 4f, false);
      b.End();
      b.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, rasterizerState: Utility.ScissorEnabled);
      Rectangle scissorRectangle = b.GraphicsDevice.ScissorRectangle;
      int x1 = this.xPositionOnScreen + 16;
      int positionOnScreen = this.yPositionOnScreen;
      for (int index = 0; index < this._farmers.Count; ++index)
      {
        Farmer farmer = this._farmers[index];
        Rectangle rectangle = scissorRectangle with
        {
          X = x1,
          Y = positionOnScreen,
          Height = num - 8,
          Width = 200
        };
        b.GraphicsDevice.ScissorRectangle = rectangle;
        FarmerRenderer.isDrawingForUI = true;
        farmer.FarmerRenderer.draw(b, new FarmerSprite.AnimationFrame((bool) (NetFieldBase<bool, NetBool>) farmer.bathingClothes ? 108 : 0, 0, false, false), (bool) (NetFieldBase<bool, NetBool>) farmer.bathingClothes ? 108 : 0, new Rectangle(0, (bool) (NetFieldBase<bool, NetBool>) farmer.bathingClothes ? 576 : 0, 16, 32), new Vector2((float) x1, (float) positionOnScreen), Vector2.Zero, 0.8f, 2, Color.White, 0.0f, 1f, farmer);
        FarmerRenderer.isDrawingForUI = false;
        b.GraphicsDevice.ScissorRectangle = scissorRectangle;
        int x2 = x1 + 80;
        int y1 = positionOnScreen + 12;
        string text1 = ChatBox.formattedUserName(farmer);
        b.DrawString(Game1.dialogueFont, text1, new Vector2((float) x2, (float) y1), Color.White);
        string userName = Game1.multiplayer.getUserName(farmer.UniqueMultiplayerID);
        if (!string.IsNullOrEmpty(userName))
        {
          int y2 = y1 + (Game1.dialogueFont.LineSpacing + 4);
          string text2 = "(" + userName + ")";
          b.DrawString(Game1.smallFont, text2, new Vector2((float) x2, (float) y2), Color.White);
        }
        positionOnScreen += num;
      }
      b.GraphicsDevice.ScissorRectangle = scissorRectangle;
      b.End();
      b.Begin(blendState: BlendState.AlphaBlend, samplerState: SamplerState.PointClamp);
    }
  }
}
