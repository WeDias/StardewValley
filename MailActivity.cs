// Decompiled with JetBrains decompiler
// Type: StardewValley.MailActivity
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

namespace StardewValley
{
  public class MailActivity : FarmActivity
  {
    public override bool AttemptActivity(Farm farm)
    {
      if (this._character.getSpouse() != null)
        this.activityPosition = Utility.PointToVector2(this._character.getSpouse().getMailboxPosition());
      else
        this.activityPosition = Utility.PointToVector2(Game1.MasterPlayer.getMailboxPosition());
      ++this.activityPosition.Y;
      this.activityDirection = 0;
      return true;
    }
  }
}
