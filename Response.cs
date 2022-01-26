// Decompiled with JetBrains decompiler
// Type: StardewValley.Response
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using Microsoft.Xna.Framework.Input;

namespace StardewValley
{
  public class Response
  {
    public string responseKey;
    public string responseText;
    public Keys hotkey;

    public Response(string responseKey, string responseText)
    {
      this.responseKey = responseKey;
      this.responseText = responseText;
    }

    public Response SetHotKey(Keys key)
    {
      this.hotkey = key;
      return this;
    }
  }
}
