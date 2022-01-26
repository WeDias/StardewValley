// Decompiled with JetBrains decompiler
// Type: Microsoft.Xna.Framework.Graphics.ViewportExtensions
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace Microsoft.Xna.Framework.Graphics
{
  public static class ViewportExtensions
  {
    public static Microsoft.Xna.Framework.Rectangle GetTitleSafeArea(this Viewport vp) => vp.TitleSafeArea;

    public static Microsoft.Xna.Framework.Rectangle ToXna(this xTile.Dimensions.Rectangle xrect) => new Microsoft.Xna.Framework.Rectangle(xrect.X, xrect.Y, xrect.Width, xrect.Height);

    public static Vector2 Size(this Viewport vp) => new Vector2((float) vp.Width, (float) vp.Height);
  }
}
