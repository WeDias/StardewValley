// Decompiled with JetBrains decompiler
// Type: StardewValley.KeyboardLayout
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System.Runtime.InteropServices;
using System.Text;

namespace StardewValley
{
  public class KeyboardLayout
  {
    private const uint KLF_ACTIVATE = 1;
    private const int KL_NAMELENGTH = 9;
    private const string LANG_EN_US = "00000409";
    private const string LANG_HE_IL = "0001101A";

    [DllImport("user32.dll")]
    private static extern long LoadKeyboardLayout(string pwszKLID, uint Flags);

    [DllImport("user32.dll")]
    private static extern long GetKeyboardLayoutName(StringBuilder pwszKLID);

    public static string getName()
    {
      StringBuilder pwszKLID = new StringBuilder(9);
      KeyboardLayout.GetKeyboardLayoutName(pwszKLID);
      return pwszKLID.ToString();
    }
  }
}
