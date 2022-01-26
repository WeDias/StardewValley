// Decompiled with JetBrains decompiler
// Type: Ionic.Crc.CRC32
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Ionic.Crc
{
  /// <summary>
  ///   Computes a CRC-32. The CRC-32 algorithm is parameterized - you
  ///   can set the polynomial and enable or disable bit
  ///   reversal. This can be used for GZIP, BZip2, or ZIP.
  /// </summary>
  /// <remarks>
  ///   This type is used internally by DotNetZip; it is generally not used
  ///   directly by applications wishing to create, read, or manipulate zip
  ///   archive files.
  /// </remarks>
  [Guid("ebc25cf6-9120-4283-b972-0e5520d0000C")]
  [ComVisible(true)]
  [ClassInterface(ClassInterfaceType.AutoDispatch)]
  public class CRC32
  {
    private uint dwPolynomial;
    private long _TotalBytesRead;
    private bool reverseBits;
    private uint[] crc32Table;
    private const int BUFFER_SIZE = 8192;
    private uint _register = uint.MaxValue;

    /// <summary>
    ///   Indicates the total number of bytes applied to the CRC.
    /// </summary>
    public long TotalBytesRead => this._TotalBytesRead;

    /// <summary>Indicates the current CRC for all blocks slurped in.</summary>
    public int Crc32Result => ~(int) this._register;

    /// <summary>Returns the CRC32 for the specified stream.</summary>
    /// <param name="input">The stream over which to calculate the CRC32</param>
    /// <returns>the CRC32 calculation</returns>
    public int GetCrc32(Stream input) => this.GetCrc32AndCopy(input, (Stream) null);

    /// <summary>
    /// Returns the CRC32 for the specified stream, and writes the input into the
    /// output stream.
    /// </summary>
    /// <param name="input">The stream over which to calculate the CRC32</param>
    /// <param name="output">The stream into which to deflate the input</param>
    /// <returns>the CRC32 calculation</returns>
    public int GetCrc32AndCopy(Stream input, Stream output)
    {
      if (input == null)
        throw new Exception("The input stream must not be null.");
      byte[] numArray = new byte[8192];
      int count1 = 8192;
      this._TotalBytesRead = 0L;
      int count2 = input.Read(numArray, 0, count1);
      output?.Write(numArray, 0, count2);
      this._TotalBytesRead += (long) count2;
      while (count2 > 0)
      {
        this.SlurpBlock(numArray, 0, count2);
        count2 = input.Read(numArray, 0, count1);
        output?.Write(numArray, 0, count2);
        this._TotalBytesRead += (long) count2;
      }
      return ~(int) this._register;
    }

    /// <summary>
    ///   Get the CRC32 for the given (word,byte) combo.  This is a
    ///   computation defined by PKzip for PKZIP 2.0 (weak) encryption.
    /// </summary>
    /// <param name="W">The word to start with.</param>
    /// <param name="B">The byte to combine it with.</param>
    /// <returns>The CRC-ized result.</returns>
    public int ComputeCrc32(int W, byte B) => this._InternalComputeCrc32((uint) W, B);

    internal int _InternalComputeCrc32(uint W, byte B) => (int) this.crc32Table[((int) W ^ (int) B) & (int) byte.MaxValue] ^ (int) (W >> 8);

    /// <summary>
    /// Update the value for the running CRC32 using the given block of bytes.
    /// This is useful when using the CRC32() class in a Stream.
    /// </summary>
    /// <param name="block">block of bytes to slurp</param>
    /// <param name="offset">starting point in the block</param>
    /// <param name="count">how many bytes within the block to slurp</param>
    public void SlurpBlock(byte[] block, int offset, int count)
    {
      if (block == null)
        throw new Exception("The data buffer must not be null.");
      for (int index1 = 0; index1 < count; ++index1)
      {
        int index2 = offset + index1;
        byte num = block[index2];
        this._register = !this.reverseBits ? this._register >> 8 ^ this.crc32Table[(int) (this._register & (uint) byte.MaxValue ^ (uint) num)] : this._register << 8 ^ this.crc32Table[(int) (this._register >> 24 ^ (uint) num)];
      }
      this._TotalBytesRead += (long) count;
    }

    /// <summary>Process one byte in the CRC.</summary>
    /// <param name="b">the byte to include into the CRC .  </param>
    public void UpdateCRC(byte b)
    {
      if (this.reverseBits)
        this._register = this._register << 8 ^ this.crc32Table[(int) (this._register >> 24 ^ (uint) b)];
      else
        this._register = this._register >> 8 ^ this.crc32Table[(int) (this._register & (uint) byte.MaxValue ^ (uint) b)];
    }

    /// <summary>Process a run of N identical bytes into the CRC.</summary>
    /// <remarks>
    ///   <para>
    ///     This method serves as an optimization for updating the CRC when a
    ///     run of identical bytes is found. Rather than passing in a buffer of
    ///     length n, containing all identical bytes b, this method accepts the
    ///     byte value and the length of the (virtual) buffer - the length of
    ///     the run.
    ///   </para>
    /// </remarks>
    /// <param name="b">the byte to include into the CRC.  </param>
    /// <param name="n">the number of times that byte should be repeated. </param>
    public void UpdateCRC(byte b, int n)
    {
      while (n-- > 0)
      {
        if (this.reverseBits)
        {
          uint num = this._register >> 24 ^ (uint) b;
          this._register = this._register << 8 ^ this.crc32Table[num >= 0U ? (int) num : (int) num + 256];
        }
        else
        {
          uint num = this._register & (uint) byte.MaxValue ^ (uint) b;
          this._register = this._register >> 8 ^ this.crc32Table[num >= 0U ? (int) num : (int) num + 256];
        }
      }
    }

    private static uint ReverseBits(uint data)
    {
      uint num1 = data;
      uint num2 = (uint) (((int) num1 & 1431655765) << 1 | (int) (num1 >> 1) & 1431655765);
      uint num3 = (uint) (((int) num2 & 858993459) << 2 | (int) (num2 >> 2) & 858993459);
      uint num4 = (uint) (((int) num3 & 252645135) << 4 | (int) (num3 >> 4) & 252645135);
      return (uint) ((int) num4 << 24 | ((int) num4 & 65280) << 8 | (int) (num4 >> 8) & 65280) | num4 >> 24;
    }

    private static byte ReverseBits(byte data)
    {
      int num1 = (int) data * 131586;
      uint num2 = 17055760;
      return (byte) ((uint) (16781313 * ((int) ((uint) num1 & num2) + (num1 << 2 & (int) num2 << 1))) >> 24);
    }

    private void GenerateLookupTable()
    {
      this.crc32Table = new uint[256];
      byte data1 = 0;
      do
      {
        uint data2 = (uint) data1;
        for (byte index = 8; index > (byte) 0; --index)
        {
          if (((int) data2 & 1) == 1)
            data2 = data2 >> 1 ^ this.dwPolynomial;
          else
            data2 >>= 1;
        }
        if (this.reverseBits)
          this.crc32Table[(int) CRC32.ReverseBits(data1)] = CRC32.ReverseBits(data2);
        else
          this.crc32Table[(int) data1] = data2;
        ++data1;
      }
      while (data1 != (byte) 0);
    }

    private uint gf2_matrix_times(uint[] matrix, uint vec)
    {
      uint num = 0;
      int index = 0;
      while (vec != 0U)
      {
        if (((int) vec & 1) == 1)
          num ^= matrix[index];
        vec >>= 1;
        ++index;
      }
      return num;
    }

    private void gf2_matrix_square(uint[] square, uint[] mat)
    {
      for (int index = 0; index < 32; ++index)
        square[index] = this.gf2_matrix_times(mat, mat[index]);
    }

    /// <summary>
    ///   Combines the given CRC32 value with the current running total.
    /// </summary>
    /// <remarks>
    ///   This is useful when using a divide-and-conquer approach to
    ///   calculating a CRC.  Multiple threads can each calculate a
    ///   CRC32 on a segment of the data, and then combine the
    ///   individual CRC32 values at the end.
    /// </remarks>
    /// <param name="crc">the crc value to be combined with this one</param>
    /// <param name="length">the length of data the CRC value was calculated on</param>
    public void Combine(int crc, int length)
    {
      uint[] numArray1 = new uint[32];
      uint[] numArray2 = new uint[32];
      if (length == 0)
        return;
      uint vec = ~this._register;
      uint num1 = (uint) crc;
      numArray2[0] = this.dwPolynomial;
      uint num2 = 1;
      for (int index = 1; index < 32; ++index)
      {
        numArray2[index] = num2;
        num2 <<= 1;
      }
      this.gf2_matrix_square(numArray1, numArray2);
      this.gf2_matrix_square(numArray2, numArray1);
      uint num3 = (uint) length;
      do
      {
        this.gf2_matrix_square(numArray1, numArray2);
        if (((int) num3 & 1) == 1)
          vec = this.gf2_matrix_times(numArray1, vec);
        uint num4 = num3 >> 1;
        if (num4 != 0U)
        {
          this.gf2_matrix_square(numArray2, numArray1);
          if (((int) num4 & 1) == 1)
            vec = this.gf2_matrix_times(numArray2, vec);
          num3 = num4 >> 1;
        }
        else
          break;
      }
      while (num3 != 0U);
      this._register = ~(vec ^ num1);
    }

    /// <summary>
    ///   Create an instance of the CRC32 class using the default settings: no
    ///   bit reversal, and a polynomial of 0xEDB88320.
    /// </summary>
    public CRC32()
      : this(false)
    {
    }

    /// <summary>
    ///   Create an instance of the CRC32 class, specifying whether to reverse
    ///   data bits or not.
    /// </summary>
    /// <param name="reverseBits">
    ///   specify true if the instance should reverse data bits.
    /// </param>
    /// <remarks>
    ///   <para>
    ///     In the CRC-32 used by BZip2, the bits are reversed. Therefore if you
    ///     want a CRC32 with compatibility with BZip2, you should pass true
    ///     here. In the CRC-32 used by GZIP and PKZIP, the bits are not
    ///     reversed; Therefore if you want a CRC32 with compatibility with
    ///     those, you should pass false.
    ///   </para>
    /// </remarks>
    public CRC32(bool reverseBits)
      : this(-306674912, reverseBits)
    {
    }

    /// <summary>
    ///   Create an instance of the CRC32 class, specifying the polynomial and
    ///   whether to reverse data bits or not.
    /// </summary>
    /// <param name="polynomial">
    ///   The polynomial to use for the CRC, expressed in the reversed (LSB)
    ///   format: the highest ordered bit in the polynomial value is the
    ///   coefficient of the 0th power; the second-highest order bit is the
    ///   coefficient of the 1 power, and so on. Expressed this way, the
    ///   polynomial for the CRC-32C used in IEEE 802.3, is 0xEDB88320.
    /// </param>
    /// <param name="reverseBits">
    ///   specify true if the instance should reverse data bits.
    /// </param>
    /// <remarks>
    ///   <para>
    ///     In the CRC-32 used by BZip2, the bits are reversed. Therefore if you
    ///     want a CRC32 with compatibility with BZip2, you should pass true
    ///     here for the <c>reverseBits</c> parameter. In the CRC-32 used by
    ///     GZIP and PKZIP, the bits are not reversed; Therefore if you want a
    ///     CRC32 with compatibility with those, you should pass false for the
    ///     <c>reverseBits</c> parameter.
    ///   </para>
    /// </remarks>
    public CRC32(int polynomial, bool reverseBits)
    {
      this.reverseBits = reverseBits;
      this.dwPolynomial = (uint) polynomial;
      this.GenerateLookupTable();
    }

    /// <summary>
    ///   Reset the CRC-32 class - clear the CRC "remainder register."
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     Use this when employing a single instance of this class to compute
    ///     multiple, distinct CRCs on multiple, distinct data blocks.
    ///   </para>
    /// </remarks>
    public void Reset() => this._register = uint.MaxValue;
  }
}
