// Decompiled with JetBrains decompiler
// Type: Ionic.Zlib.DeflateManager
// Assembly: Stardew Valley, Version=1.5.6.22018, Culture=neutral, PublicKeyToken=null
// MVID: BEBB6D18-4941-4529-AC12-B54F0C61CC20
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley\Stardew Valley.dll

using System;

namespace Ionic.Zlib
{
  internal sealed class DeflateManager
  {
    private static readonly int MEM_LEVEL_MAX = 9;
    private static readonly int MEM_LEVEL_DEFAULT = 8;
    private DeflateManager.CompressFunc DeflateFunction;
    private static readonly string[] _ErrorMessage = new string[10]
    {
      "need dictionary",
      "stream end",
      "",
      "file error",
      "stream error",
      "data error",
      "insufficient memory",
      "buffer error",
      "incompatible version",
      ""
    };
    private static readonly int PRESET_DICT = 32;
    private static readonly int INIT_STATE = 42;
    private static readonly int BUSY_STATE = 113;
    private static readonly int FINISH_STATE = 666;
    private static readonly int Z_DEFLATED = 8;
    private static readonly int STORED_BLOCK = 0;
    private static readonly int STATIC_TREES = 1;
    private static readonly int DYN_TREES = 2;
    private static readonly int Z_BINARY = 0;
    private static readonly int Z_ASCII = 1;
    private static readonly int Z_UNKNOWN = 2;
    private static readonly int Buf_size = 16;
    private static readonly int MIN_MATCH = 3;
    private static readonly int MAX_MATCH = 258;
    private static readonly int MIN_LOOKAHEAD = DeflateManager.MAX_MATCH + DeflateManager.MIN_MATCH + 1;
    private static readonly int HEAP_SIZE = 2 * InternalConstants.L_CODES + 1;
    private static readonly int END_BLOCK = 256;
    internal ZlibCodec _codec;
    internal int status;
    internal byte[] pending;
    internal int nextPending;
    internal int pendingCount;
    internal sbyte data_type;
    internal int last_flush;
    internal int w_size;
    internal int w_bits;
    internal int w_mask;
    internal byte[] window;
    internal int window_size;
    internal short[] prev;
    internal short[] head;
    internal int ins_h;
    internal int hash_size;
    internal int hash_bits;
    internal int hash_mask;
    internal int hash_shift;
    internal int block_start;
    private DeflateManager.Config config;
    internal int match_length;
    internal int prev_match;
    internal int match_available;
    internal int strstart;
    internal int match_start;
    internal int lookahead;
    internal int prev_length;
    internal CompressionLevel compressionLevel;
    internal CompressionStrategy compressionStrategy;
    internal short[] dyn_ltree;
    internal short[] dyn_dtree;
    internal short[] bl_tree;
    internal Tree treeLiterals = new Tree();
    internal Tree treeDistances = new Tree();
    internal Tree treeBitLengths = new Tree();
    internal short[] bl_count = new short[InternalConstants.MAX_BITS + 1];
    internal int[] heap = new int[2 * InternalConstants.L_CODES + 1];
    internal int heap_len;
    internal int heap_max;
    internal sbyte[] depth = new sbyte[2 * InternalConstants.L_CODES + 1];
    internal int _lengthOffset;
    internal int lit_bufsize;
    internal int last_lit;
    internal int _distanceOffset;
    internal int opt_len;
    internal int static_len;
    internal int matches;
    internal int last_eob_len;
    internal short bi_buf;
    internal int bi_valid;
    private bool Rfc1950BytesEmitted;
    private bool _WantRfc1950HeaderBytes = true;

    internal DeflateManager()
    {
      this.dyn_ltree = new short[DeflateManager.HEAP_SIZE * 2];
      this.dyn_dtree = new short[(2 * InternalConstants.D_CODES + 1) * 2];
      this.bl_tree = new short[(2 * InternalConstants.BL_CODES + 1) * 2];
    }

    private void _InitializeLazyMatch()
    {
      this.window_size = 2 * this.w_size;
      Array.Clear((Array) this.head, 0, this.hash_size);
      this.config = DeflateManager.Config.Lookup(this.compressionLevel);
      this.SetDeflater();
      this.strstart = 0;
      this.block_start = 0;
      this.lookahead = 0;
      this.match_length = this.prev_length = DeflateManager.MIN_MATCH - 1;
      this.match_available = 0;
      this.ins_h = 0;
    }

    private void _InitializeTreeData()
    {
      this.treeLiterals.dyn_tree = this.dyn_ltree;
      this.treeLiterals.staticTree = StaticTree.Literals;
      this.treeDistances.dyn_tree = this.dyn_dtree;
      this.treeDistances.staticTree = StaticTree.Distances;
      this.treeBitLengths.dyn_tree = this.bl_tree;
      this.treeBitLengths.staticTree = StaticTree.BitLengths;
      this.bi_buf = (short) 0;
      this.bi_valid = 0;
      this.last_eob_len = 8;
      this._InitializeBlocks();
    }

    internal void _InitializeBlocks()
    {
      for (int index = 0; index < InternalConstants.L_CODES; ++index)
        this.dyn_ltree[index * 2] = (short) 0;
      for (int index = 0; index < InternalConstants.D_CODES; ++index)
        this.dyn_dtree[index * 2] = (short) 0;
      for (int index = 0; index < InternalConstants.BL_CODES; ++index)
        this.bl_tree[index * 2] = (short) 0;
      this.dyn_ltree[DeflateManager.END_BLOCK * 2] = (short) 1;
      this.opt_len = this.static_len = 0;
      this.last_lit = this.matches = 0;
    }

    internal void pqdownheap(short[] tree, int k)
    {
      int n = this.heap[k];
      for (int index = k << 1; index <= this.heap_len; index <<= 1)
      {
        if (index < this.heap_len && DeflateManager._IsSmaller(tree, this.heap[index + 1], this.heap[index], this.depth))
          ++index;
        if (!DeflateManager._IsSmaller(tree, n, this.heap[index], this.depth))
        {
          this.heap[k] = this.heap[index];
          k = index;
        }
        else
          break;
      }
      this.heap[k] = n;
    }

    internal static bool _IsSmaller(short[] tree, int n, int m, sbyte[] depth)
    {
      short num1 = tree[n * 2];
      short num2 = tree[m * 2];
      if ((int) num1 < (int) num2)
        return true;
      return (int) num1 == (int) num2 && (int) depth[n] <= (int) depth[m];
    }

    internal void scan_tree(short[] tree, int max_code)
    {
      int num1 = -1;
      int num2 = (int) tree[1];
      int num3 = 0;
      int num4 = 7;
      int num5 = 4;
      if (num2 == 0)
      {
        num4 = 138;
        num5 = 3;
      }
      tree[(max_code + 1) * 2 + 1] = short.MaxValue;
      for (int index = 0; index <= max_code; ++index)
      {
        int num6 = num2;
        num2 = (int) tree[(index + 1) * 2 + 1];
        if (++num3 >= num4 || num6 != num2)
        {
          if (num3 < num5)
            this.bl_tree[num6 * 2] = (short) ((int) this.bl_tree[num6 * 2] + num3);
          else if (num6 != 0)
          {
            if (num6 != num1)
              ++this.bl_tree[num6 * 2];
            ++this.bl_tree[InternalConstants.REP_3_6 * 2];
          }
          else if (num3 <= 10)
            ++this.bl_tree[InternalConstants.REPZ_3_10 * 2];
          else
            ++this.bl_tree[InternalConstants.REPZ_11_138 * 2];
          num3 = 0;
          num1 = num6;
          if (num2 == 0)
          {
            num4 = 138;
            num5 = 3;
          }
          else if (num6 == num2)
          {
            num4 = 6;
            num5 = 3;
          }
          else
          {
            num4 = 7;
            num5 = 4;
          }
        }
      }
    }

    internal int build_bl_tree()
    {
      this.scan_tree(this.dyn_ltree, this.treeLiterals.max_code);
      this.scan_tree(this.dyn_dtree, this.treeDistances.max_code);
      this.treeBitLengths.build_tree(this);
      int index = InternalConstants.BL_CODES - 1;
      while (index >= 3 && this.bl_tree[(int) Tree.bl_order[index] * 2 + 1] == (short) 0)
        --index;
      this.opt_len += 3 * (index + 1) + 5 + 5 + 4;
      return index;
    }

    internal void send_all_trees(int lcodes, int dcodes, int blcodes)
    {
      this.send_bits(lcodes - 257, 5);
      this.send_bits(dcodes - 1, 5);
      this.send_bits(blcodes - 4, 4);
      for (int index = 0; index < blcodes; ++index)
        this.send_bits((int) this.bl_tree[(int) Tree.bl_order[index] * 2 + 1], 3);
      this.send_tree(this.dyn_ltree, lcodes - 1);
      this.send_tree(this.dyn_dtree, dcodes - 1);
    }

    internal void send_tree(short[] tree, int max_code)
    {
      int num1 = -1;
      int num2 = (int) tree[1];
      int num3 = 0;
      int num4 = 7;
      int num5 = 4;
      if (num2 == 0)
      {
        num4 = 138;
        num5 = 3;
      }
      for (int index = 0; index <= max_code; ++index)
      {
        int c = num2;
        num2 = (int) tree[(index + 1) * 2 + 1];
        if (++num3 >= num4 || c != num2)
        {
          if (num3 < num5)
          {
            do
            {
              this.send_code(c, this.bl_tree);
            }
            while (--num3 != 0);
          }
          else if (c != 0)
          {
            if (c != num1)
            {
              this.send_code(c, this.bl_tree);
              --num3;
            }
            this.send_code(InternalConstants.REP_3_6, this.bl_tree);
            this.send_bits(num3 - 3, 2);
          }
          else if (num3 <= 10)
          {
            this.send_code(InternalConstants.REPZ_3_10, this.bl_tree);
            this.send_bits(num3 - 3, 3);
          }
          else
          {
            this.send_code(InternalConstants.REPZ_11_138, this.bl_tree);
            this.send_bits(num3 - 11, 7);
          }
          num3 = 0;
          num1 = c;
          if (num2 == 0)
          {
            num4 = 138;
            num5 = 3;
          }
          else if (c == num2)
          {
            num4 = 6;
            num5 = 3;
          }
          else
          {
            num4 = 7;
            num5 = 4;
          }
        }
      }
    }

    private void put_bytes(byte[] p, int start, int len)
    {
      Array.Copy((Array) p, start, (Array) this.pending, this.pendingCount, len);
      this.pendingCount += len;
    }

    internal void send_code(int c, short[] tree)
    {
      int index = c * 2;
      this.send_bits((int) tree[index] & (int) ushort.MaxValue, (int) tree[index + 1] & (int) ushort.MaxValue);
    }

    internal void send_bits(int value, int length)
    {
      int num = length;
      if (this.bi_valid > DeflateManager.Buf_size - num)
      {
        this.bi_buf |= (short) (value << this.bi_valid & (int) ushort.MaxValue);
        this.pending[this.pendingCount++] = (byte) this.bi_buf;
        this.pending[this.pendingCount++] = (byte) ((uint) this.bi_buf >> 8);
        this.bi_buf = (short) ((uint) value >> DeflateManager.Buf_size - this.bi_valid);
        this.bi_valid += num - DeflateManager.Buf_size;
      }
      else
      {
        this.bi_buf |= (short) (value << this.bi_valid & (int) ushort.MaxValue);
        this.bi_valid += num;
      }
    }

    internal void _tr_align()
    {
      this.send_bits(DeflateManager.STATIC_TREES << 1, 3);
      this.send_code(DeflateManager.END_BLOCK, StaticTree.lengthAndLiteralsTreeCodes);
      this.bi_flush();
      if (1 + this.last_eob_len + 10 - this.bi_valid < 9)
      {
        this.send_bits(DeflateManager.STATIC_TREES << 1, 3);
        this.send_code(DeflateManager.END_BLOCK, StaticTree.lengthAndLiteralsTreeCodes);
        this.bi_flush();
      }
      this.last_eob_len = 7;
    }

    internal bool _tr_tally(int dist, int lc)
    {
      this.pending[this._distanceOffset + this.last_lit * 2] = (byte) ((uint) dist >> 8);
      this.pending[this._distanceOffset + this.last_lit * 2 + 1] = (byte) dist;
      this.pending[this._lengthOffset + this.last_lit] = (byte) lc;
      ++this.last_lit;
      if (dist == 0)
      {
        ++this.dyn_ltree[lc * 2];
      }
      else
      {
        ++this.matches;
        --dist;
        ++this.dyn_ltree[((int) Tree.LengthCode[lc] + InternalConstants.LITERALS + 1) * 2];
        ++this.dyn_dtree[Tree.DistanceCode(dist) * 2];
      }
      if ((this.last_lit & 8191) == 0 && this.compressionLevel > CompressionLevel.Level2)
      {
        int num1 = this.last_lit << 3;
        int num2 = this.strstart - this.block_start;
        for (int index = 0; index < InternalConstants.D_CODES; ++index)
          num1 = (int) ((long) num1 + (long) this.dyn_dtree[index * 2] * (5L + (long) Tree.ExtraDistanceBits[index]));
        int num3 = num1 >> 3;
        if (this.matches < this.last_lit / 2 && num3 < num2 / 2)
          return true;
      }
      return this.last_lit == this.lit_bufsize - 1 || this.last_lit == this.lit_bufsize;
    }

    internal void send_compressed_block(short[] ltree, short[] dtree)
    {
      int num1 = 0;
      if (this.last_lit != 0)
      {
        do
        {
          int index1 = this._distanceOffset + num1 * 2;
          int num2 = (int) this.pending[index1] << 8 & 65280 | (int) this.pending[index1 + 1] & (int) byte.MaxValue;
          int c1 = (int) this.pending[this._lengthOffset + num1] & (int) byte.MaxValue;
          ++num1;
          if (num2 == 0)
          {
            this.send_code(c1, ltree);
          }
          else
          {
            int index2 = (int) Tree.LengthCode[c1];
            this.send_code(index2 + InternalConstants.LITERALS + 1, ltree);
            int extraLengthBit = Tree.ExtraLengthBits[index2];
            if (extraLengthBit != 0)
              this.send_bits(c1 - Tree.LengthBase[index2], extraLengthBit);
            int dist = num2 - 1;
            int c2 = Tree.DistanceCode(dist);
            this.send_code(c2, dtree);
            int extraDistanceBit = Tree.ExtraDistanceBits[c2];
            if (extraDistanceBit != 0)
              this.send_bits(dist - Tree.DistanceBase[c2], extraDistanceBit);
          }
        }
        while (num1 < this.last_lit);
      }
      this.send_code(DeflateManager.END_BLOCK, ltree);
      this.last_eob_len = (int) ltree[DeflateManager.END_BLOCK * 2 + 1];
    }

    internal void set_data_type()
    {
      int num1 = 0;
      int num2 = 0;
      int num3 = 0;
      for (; num1 < 7; ++num1)
        num3 += (int) this.dyn_ltree[num1 * 2];
      for (; num1 < 128; ++num1)
        num2 += (int) this.dyn_ltree[num1 * 2];
      for (; num1 < InternalConstants.LITERALS; ++num1)
        num3 += (int) this.dyn_ltree[num1 * 2];
      this.data_type = num3 > num2 >> 2 ? (sbyte) DeflateManager.Z_BINARY : (sbyte) DeflateManager.Z_ASCII;
    }

    internal void bi_flush()
    {
      if (this.bi_valid == 16)
      {
        this.pending[this.pendingCount++] = (byte) this.bi_buf;
        this.pending[this.pendingCount++] = (byte) ((uint) this.bi_buf >> 8);
        this.bi_buf = (short) 0;
        this.bi_valid = 0;
      }
      else
      {
        if (this.bi_valid < 8)
          return;
        this.pending[this.pendingCount++] = (byte) this.bi_buf;
        this.bi_buf >>= 8;
        this.bi_valid -= 8;
      }
    }

    internal void bi_windup()
    {
      if (this.bi_valid > 8)
      {
        this.pending[this.pendingCount++] = (byte) this.bi_buf;
        this.pending[this.pendingCount++] = (byte) ((uint) this.bi_buf >> 8);
      }
      else if (this.bi_valid > 0)
        this.pending[this.pendingCount++] = (byte) this.bi_buf;
      this.bi_buf = (short) 0;
      this.bi_valid = 0;
    }

    internal void copy_block(int buf, int len, bool header)
    {
      this.bi_windup();
      this.last_eob_len = 8;
      if (header)
      {
        this.pending[this.pendingCount++] = (byte) len;
        this.pending[this.pendingCount++] = (byte) (len >> 8);
        this.pending[this.pendingCount++] = (byte) ~len;
        this.pending[this.pendingCount++] = (byte) (~len >> 8);
      }
      this.put_bytes(this.window, buf, len);
    }

    internal void flush_block_only(bool eof)
    {
      this._tr_flush_block(this.block_start >= 0 ? this.block_start : -1, this.strstart - this.block_start, eof);
      this.block_start = this.strstart;
      this._codec.flush_pending();
    }

    internal BlockState DeflateNone(FlushType flush)
    {
      int num1 = (int) ushort.MaxValue;
      if (num1 > this.pending.Length - 5)
        num1 = this.pending.Length - 5;
      do
      {
        do
        {
          if (this.lookahead <= 1)
          {
            this._fillWindow();
            if (this.lookahead == 0 && flush == FlushType.None)
              return BlockState.NeedMore;
            if (this.lookahead == 0)
              goto label_12;
          }
          this.strstart += this.lookahead;
          this.lookahead = 0;
          int num2 = this.block_start + num1;
          if (this.strstart == 0 || this.strstart >= num2)
          {
            this.lookahead = this.strstart - num2;
            this.strstart = num2;
            this.flush_block_only(false);
            if (this._codec.AvailableBytesOut == 0)
              return BlockState.NeedMore;
          }
        }
        while (this.strstart - this.block_start < this.w_size - DeflateManager.MIN_LOOKAHEAD);
        this.flush_block_only(false);
      }
      while (this._codec.AvailableBytesOut != 0);
      return BlockState.NeedMore;
label_12:
      this.flush_block_only(flush == FlushType.Finish);
      return this._codec.AvailableBytesOut == 0 ? (flush != FlushType.Finish ? BlockState.NeedMore : BlockState.FinishStarted) : (flush != FlushType.Finish ? BlockState.BlockDone : BlockState.FinishDone);
    }

    internal void _tr_stored_block(int buf, int stored_len, bool eof)
    {
      this.send_bits((DeflateManager.STORED_BLOCK << 1) + (eof ? 1 : 0), 3);
      this.copy_block(buf, stored_len, true);
    }

    internal void _tr_flush_block(int buf, int stored_len, bool eof)
    {
      int num1 = 0;
      int num2;
      int num3;
      if (this.compressionLevel > CompressionLevel.None)
      {
        if ((int) this.data_type == DeflateManager.Z_UNKNOWN)
          this.set_data_type();
        this.treeLiterals.build_tree(this);
        this.treeDistances.build_tree(this);
        num1 = this.build_bl_tree();
        num2 = this.opt_len + 3 + 7 >> 3;
        num3 = this.static_len + 3 + 7 >> 3;
        if (num3 <= num2)
          num2 = num3;
      }
      else
        num2 = num3 = stored_len + 5;
      if (stored_len + 4 <= num2 && buf != -1)
        this._tr_stored_block(buf, stored_len, eof);
      else if (num3 == num2)
      {
        this.send_bits((DeflateManager.STATIC_TREES << 1) + (eof ? 1 : 0), 3);
        this.send_compressed_block(StaticTree.lengthAndLiteralsTreeCodes, StaticTree.distTreeCodes);
      }
      else
      {
        this.send_bits((DeflateManager.DYN_TREES << 1) + (eof ? 1 : 0), 3);
        this.send_all_trees(this.treeLiterals.max_code + 1, this.treeDistances.max_code + 1, num1 + 1);
        this.send_compressed_block(this.dyn_ltree, this.dyn_dtree);
      }
      this._InitializeBlocks();
      if (!eof)
        return;
      this.bi_windup();
    }

    private void _fillWindow()
    {
      do
      {
        int size = this.window_size - this.lookahead - this.strstart;
        if (size == 0 && this.strstart == 0 && this.lookahead == 0)
          size = this.w_size;
        else if (size == -1)
          --size;
        else if (this.strstart >= this.w_size + this.w_size - DeflateManager.MIN_LOOKAHEAD)
        {
          Array.Copy((Array) this.window, this.w_size, (Array) this.window, 0, this.w_size);
          this.match_start -= this.w_size;
          this.strstart -= this.w_size;
          this.block_start -= this.w_size;
          int hashSize = this.hash_size;
          int index1 = hashSize;
          do
          {
            int num = (int) this.head[--index1] & (int) ushort.MaxValue;
            this.head[index1] = num >= this.w_size ? (short) (num - this.w_size) : (short) 0;
          }
          while (--hashSize != 0);
          int wSize = this.w_size;
          int index2 = wSize;
          do
          {
            int num = (int) this.prev[--index2] & (int) ushort.MaxValue;
            this.prev[index2] = num >= this.w_size ? (short) (num - this.w_size) : (short) 0;
          }
          while (--wSize != 0);
          size += this.w_size;
        }
        if (this._codec.AvailableBytesIn == 0)
          break;
        this.lookahead += this._codec.read_buf(this.window, this.strstart + this.lookahead, size);
        if (this.lookahead >= DeflateManager.MIN_MATCH)
        {
          this.ins_h = (int) this.window[this.strstart] & (int) byte.MaxValue;
          this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[this.strstart + 1] & (int) byte.MaxValue) & this.hash_mask;
        }
      }
      while (this.lookahead < DeflateManager.MIN_LOOKAHEAD && this._codec.AvailableBytesIn != 0);
    }

    internal BlockState DeflateFast(FlushType flush)
    {
      int cur_match = 0;
      do
      {
        bool flag;
        do
        {
          if (this.lookahead < DeflateManager.MIN_LOOKAHEAD)
          {
            this._fillWindow();
            if (this.lookahead < DeflateManager.MIN_LOOKAHEAD && flush == FlushType.None)
              return BlockState.NeedMore;
            if (this.lookahead == 0)
              goto label_19;
          }
          if (this.lookahead >= DeflateManager.MIN_MATCH)
          {
            this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[this.strstart + (DeflateManager.MIN_MATCH - 1)] & (int) byte.MaxValue) & this.hash_mask;
            cur_match = (int) this.head[this.ins_h] & (int) ushort.MaxValue;
            this.prev[this.strstart & this.w_mask] = this.head[this.ins_h];
            this.head[this.ins_h] = (short) this.strstart;
          }
          if (cur_match != 0 && (this.strstart - cur_match & (int) ushort.MaxValue) <= this.w_size - DeflateManager.MIN_LOOKAHEAD && this.compressionStrategy != CompressionStrategy.HuffmanOnly)
            this.match_length = this.longest_match(cur_match);
          if (this.match_length >= DeflateManager.MIN_MATCH)
          {
            flag = this._tr_tally(this.strstart - this.match_start, this.match_length - DeflateManager.MIN_MATCH);
            this.lookahead -= this.match_length;
            if (this.match_length <= this.config.MaxLazy && this.lookahead >= DeflateManager.MIN_MATCH)
            {
              --this.match_length;
              do
              {
                ++this.strstart;
                this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[this.strstart + (DeflateManager.MIN_MATCH - 1)] & (int) byte.MaxValue) & this.hash_mask;
                cur_match = (int) this.head[this.ins_h] & (int) ushort.MaxValue;
                this.prev[this.strstart & this.w_mask] = this.head[this.ins_h];
                this.head[this.ins_h] = (short) this.strstart;
              }
              while (--this.match_length != 0);
              ++this.strstart;
            }
            else
            {
              this.strstart += this.match_length;
              this.match_length = 0;
              this.ins_h = (int) this.window[this.strstart] & (int) byte.MaxValue;
              this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[this.strstart + 1] & (int) byte.MaxValue) & this.hash_mask;
            }
          }
          else
          {
            flag = this._tr_tally(0, (int) this.window[this.strstart] & (int) byte.MaxValue);
            --this.lookahead;
            ++this.strstart;
          }
        }
        while (!flag);
        this.flush_block_only(false);
      }
      while (this._codec.AvailableBytesOut != 0);
      return BlockState.NeedMore;
label_19:
      this.flush_block_only(flush == FlushType.Finish);
      return this._codec.AvailableBytesOut == 0 ? (flush == FlushType.Finish ? BlockState.FinishStarted : BlockState.NeedMore) : (flush != FlushType.Finish ? BlockState.BlockDone : BlockState.FinishDone);
    }

    internal BlockState DeflateSlow(FlushType flush)
    {
      int cur_match = 0;
      while (true)
      {
        do
        {
          if (this.lookahead < DeflateManager.MIN_LOOKAHEAD)
          {
            this._fillWindow();
            if (this.lookahead < DeflateManager.MIN_LOOKAHEAD && flush == FlushType.None)
              return BlockState.NeedMore;
            if (this.lookahead == 0)
              goto label_26;
          }
          if (this.lookahead >= DeflateManager.MIN_MATCH)
          {
            this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[this.strstart + (DeflateManager.MIN_MATCH - 1)] & (int) byte.MaxValue) & this.hash_mask;
            cur_match = (int) this.head[this.ins_h] & (int) ushort.MaxValue;
            this.prev[this.strstart & this.w_mask] = this.head[this.ins_h];
            this.head[this.ins_h] = (short) this.strstart;
          }
          this.prev_length = this.match_length;
          this.prev_match = this.match_start;
          this.match_length = DeflateManager.MIN_MATCH - 1;
          if (cur_match != 0 && this.prev_length < this.config.MaxLazy && (this.strstart - cur_match & (int) ushort.MaxValue) <= this.w_size - DeflateManager.MIN_LOOKAHEAD)
          {
            if (this.compressionStrategy != CompressionStrategy.HuffmanOnly)
              this.match_length = this.longest_match(cur_match);
            if (this.match_length <= 5 && (this.compressionStrategy == CompressionStrategy.Filtered || this.match_length == DeflateManager.MIN_MATCH && this.strstart - this.match_start > 4096))
              this.match_length = DeflateManager.MIN_MATCH - 1;
          }
          if (this.prev_length >= DeflateManager.MIN_MATCH && this.match_length <= this.prev_length)
          {
            int num = this.strstart + this.lookahead - DeflateManager.MIN_MATCH;
            bool flag = this._tr_tally(this.strstart - 1 - this.prev_match, this.prev_length - DeflateManager.MIN_MATCH);
            this.lookahead -= this.prev_length - 1;
            this.prev_length -= 2;
            do
            {
              if (++this.strstart <= num)
              {
                this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[this.strstart + (DeflateManager.MIN_MATCH - 1)] & (int) byte.MaxValue) & this.hash_mask;
                cur_match = (int) this.head[this.ins_h] & (int) ushort.MaxValue;
                this.prev[this.strstart & this.w_mask] = this.head[this.ins_h];
                this.head[this.ins_h] = (short) this.strstart;
              }
            }
            while (--this.prev_length != 0);
            this.match_available = 0;
            this.match_length = DeflateManager.MIN_MATCH - 1;
            ++this.strstart;
            if (flag)
            {
              this.flush_block_only(false);
              if (this._codec.AvailableBytesOut == 0)
                return BlockState.NeedMore;
            }
          }
          else if (this.match_available != 0)
          {
            if (this._tr_tally(0, (int) this.window[this.strstart - 1] & (int) byte.MaxValue))
              this.flush_block_only(false);
            ++this.strstart;
            --this.lookahead;
          }
          else
            goto label_25;
        }
        while (this._codec.AvailableBytesOut != 0);
        break;
label_25:
        this.match_available = 1;
        ++this.strstart;
        --this.lookahead;
      }
      return BlockState.NeedMore;
label_26:
      if (this.match_available != 0)
      {
        this._tr_tally(0, (int) this.window[this.strstart - 1] & (int) byte.MaxValue);
        this.match_available = 0;
      }
      this.flush_block_only(flush == FlushType.Finish);
      return this._codec.AvailableBytesOut == 0 ? (flush == FlushType.Finish ? BlockState.FinishStarted : BlockState.NeedMore) : (flush != FlushType.Finish ? BlockState.BlockDone : BlockState.FinishDone);
    }

    internal int longest_match(int cur_match)
    {
      int maxChainLength = this.config.MaxChainLength;
      int index1 = this.strstart;
      int num1 = this.prev_length;
      int num2 = this.strstart > this.w_size - DeflateManager.MIN_LOOKAHEAD ? this.strstart - (this.w_size - DeflateManager.MIN_LOOKAHEAD) : 0;
      int num3 = this.config.NiceLength;
      int wMask = this.w_mask;
      int num4 = this.strstart + DeflateManager.MAX_MATCH;
      byte num5 = this.window[index1 + num1 - 1];
      byte num6 = this.window[index1 + num1];
      if (this.prev_length >= this.config.GoodLength)
        maxChainLength >>= 2;
      if (num3 > this.lookahead)
        num3 = this.lookahead;
      do
      {
        int index2 = cur_match;
        int num7;
        if ((int) this.window[index2 + num1] == (int) num6 && (int) this.window[index2 + num1 - 1] == (int) num5 && (int) this.window[index2] == (int) this.window[index1] && (int) this.window[num7 = index2 + 1] == (int) this.window[index1 + 1])
        {
          int num8 = index1 + 2;
          int num9 = num7 + 1;
          int num10;
          int num11;
          int num12;
          int num13;
          int num14;
          int num15;
          int num16;
          do
            ;
          while ((int) this.window[++num8] == (int) this.window[num10 = num9 + 1] && (int) this.window[++num8] == (int) this.window[num11 = num10 + 1] && (int) this.window[++num8] == (int) this.window[num12 = num11 + 1] && (int) this.window[++num8] == (int) this.window[num13 = num12 + 1] && (int) this.window[++num8] == (int) this.window[num14 = num13 + 1] && (int) this.window[++num8] == (int) this.window[num15 = num14 + 1] && (int) this.window[++num8] == (int) this.window[num16 = num15 + 1] && (int) this.window[++num8] == (int) this.window[num9 = num16 + 1] && num8 < num4);
          int num17 = DeflateManager.MAX_MATCH - (num4 - num8);
          index1 = num4 - DeflateManager.MAX_MATCH;
          if (num17 > num1)
          {
            this.match_start = cur_match;
            num1 = num17;
            if (num17 < num3)
            {
              num5 = this.window[index1 + num1 - 1];
              num6 = this.window[index1 + num1];
            }
            else
              break;
          }
        }
      }
      while ((cur_match = (int) this.prev[cur_match & wMask] & (int) ushort.MaxValue) > num2 && --maxChainLength != 0);
      return num1 <= this.lookahead ? num1 : this.lookahead;
    }

    internal bool WantRfc1950HeaderBytes
    {
      get => this._WantRfc1950HeaderBytes;
      set => this._WantRfc1950HeaderBytes = value;
    }

    internal int Initialize(ZlibCodec codec, CompressionLevel level) => this.Initialize(codec, level, 15);

    internal int Initialize(ZlibCodec codec, CompressionLevel level, int bits) => this.Initialize(codec, level, bits, DeflateManager.MEM_LEVEL_DEFAULT, CompressionStrategy.Default);

    internal int Initialize(
      ZlibCodec codec,
      CompressionLevel level,
      int bits,
      CompressionStrategy compressionStrategy)
    {
      return this.Initialize(codec, level, bits, DeflateManager.MEM_LEVEL_DEFAULT, compressionStrategy);
    }

    internal int Initialize(
      ZlibCodec codec,
      CompressionLevel level,
      int windowBits,
      int memLevel,
      CompressionStrategy strategy)
    {
      this._codec = codec;
      this._codec.Message = (string) null;
      if (windowBits < 9 || windowBits > 15)
        throw new ZlibException("windowBits must be in the range 9..15.");
      if (memLevel < 1 || memLevel > DeflateManager.MEM_LEVEL_MAX)
        throw new ZlibException(string.Format("memLevel must be in the range 1.. {0}", (object) DeflateManager.MEM_LEVEL_MAX));
      this._codec.dstate = this;
      this.w_bits = windowBits;
      this.w_size = 1 << this.w_bits;
      this.w_mask = this.w_size - 1;
      this.hash_bits = memLevel + 7;
      this.hash_size = 1 << this.hash_bits;
      this.hash_mask = this.hash_size - 1;
      this.hash_shift = (this.hash_bits + DeflateManager.MIN_MATCH - 1) / DeflateManager.MIN_MATCH;
      this.window = new byte[this.w_size * 2];
      this.prev = new short[this.w_size];
      this.head = new short[this.hash_size];
      this.lit_bufsize = 1 << memLevel + 6;
      this.pending = new byte[this.lit_bufsize * 4];
      this._distanceOffset = this.lit_bufsize;
      this._lengthOffset = 3 * this.lit_bufsize;
      this.compressionLevel = level;
      this.compressionStrategy = strategy;
      this.Reset();
      return 0;
    }

    internal void Reset()
    {
      this._codec.TotalBytesIn = this._codec.TotalBytesOut = 0L;
      this._codec.Message = (string) null;
      this.pendingCount = 0;
      this.nextPending = 0;
      this.Rfc1950BytesEmitted = false;
      this.status = this.WantRfc1950HeaderBytes ? DeflateManager.INIT_STATE : DeflateManager.BUSY_STATE;
      this._codec._Adler32 = Adler.Adler32(0U, (byte[]) null, 0, 0);
      this.last_flush = 0;
      this._InitializeTreeData();
      this._InitializeLazyMatch();
    }

    internal int End()
    {
      if (this.status != DeflateManager.INIT_STATE && this.status != DeflateManager.BUSY_STATE && this.status != DeflateManager.FINISH_STATE)
        return -2;
      this.pending = (byte[]) null;
      this.head = (short[]) null;
      this.prev = (short[]) null;
      this.window = (byte[]) null;
      return this.status != DeflateManager.BUSY_STATE ? 0 : -3;
    }

    private void SetDeflater()
    {
      switch (this.config.Flavor)
      {
        case DeflateFlavor.Store:
          this.DeflateFunction = new DeflateManager.CompressFunc(this.DeflateNone);
          break;
        case DeflateFlavor.Fast:
          this.DeflateFunction = new DeflateManager.CompressFunc(this.DeflateFast);
          break;
        case DeflateFlavor.Slow:
          this.DeflateFunction = new DeflateManager.CompressFunc(this.DeflateSlow);
          break;
      }
    }

    internal int SetParams(CompressionLevel level, CompressionStrategy strategy)
    {
      int num = 0;
      if (this.compressionLevel != level)
      {
        DeflateManager.Config config = DeflateManager.Config.Lookup(level);
        if (config.Flavor != this.config.Flavor && this._codec.TotalBytesIn != 0L)
          num = this._codec.Deflate(FlushType.Partial);
        this.compressionLevel = level;
        this.config = config;
        this.SetDeflater();
      }
      this.compressionStrategy = strategy;
      return num;
    }

    internal int SetDictionary(byte[] dictionary)
    {
      int length = dictionary.Length;
      int sourceIndex = 0;
      if (dictionary == null || this.status != DeflateManager.INIT_STATE)
        throw new ZlibException("Stream error.");
      this._codec._Adler32 = Adler.Adler32(this._codec._Adler32, dictionary, 0, dictionary.Length);
      if (length < DeflateManager.MIN_MATCH)
        return 0;
      if (length > this.w_size - DeflateManager.MIN_LOOKAHEAD)
      {
        length = this.w_size - DeflateManager.MIN_LOOKAHEAD;
        sourceIndex = dictionary.Length - length;
      }
      Array.Copy((Array) dictionary, sourceIndex, (Array) this.window, 0, length);
      this.strstart = length;
      this.block_start = length;
      this.ins_h = (int) this.window[0] & (int) byte.MaxValue;
      this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[1] & (int) byte.MaxValue) & this.hash_mask;
      for (int index = 0; index <= length - DeflateManager.MIN_MATCH; ++index)
      {
        this.ins_h = (this.ins_h << this.hash_shift ^ (int) this.window[index + (DeflateManager.MIN_MATCH - 1)] & (int) byte.MaxValue) & this.hash_mask;
        this.prev[index & this.w_mask] = this.head[this.ins_h];
        this.head[this.ins_h] = (short) index;
      }
      return 0;
    }

    internal int Deflate(FlushType flush)
    {
      if (this._codec.OutputBuffer == null || this._codec.InputBuffer == null && this._codec.AvailableBytesIn != 0 || this.status == DeflateManager.FINISH_STATE && flush != FlushType.Finish)
      {
        this._codec.Message = DeflateManager._ErrorMessage[4];
        throw new ZlibException(string.Format("Something is fishy. [{0}]", (object) this._codec.Message));
      }
      if (this._codec.AvailableBytesOut == 0)
      {
        this._codec.Message = DeflateManager._ErrorMessage[7];
        throw new ZlibException("OutputBuffer is full (AvailableBytesOut == 0)");
      }
      int lastFlush = this.last_flush;
      this.last_flush = (int) flush;
      if (this.status == DeflateManager.INIT_STATE)
      {
        int num1 = DeflateManager.Z_DEFLATED + (this.w_bits - 8 << 4) << 8;
        int num2 = (int) (this.compressionLevel - 1 & (CompressionLevel) 255) >> 1;
        if (num2 > 3)
          num2 = 3;
        int num3 = num1 | num2 << 6;
        if (this.strstart != 0)
          num3 |= DeflateManager.PRESET_DICT;
        int num4 = num3 + (31 - num3 % 31);
        this.status = DeflateManager.BUSY_STATE;
        this.pending[this.pendingCount++] = (byte) (num4 >> 8);
        this.pending[this.pendingCount++] = (byte) num4;
        if (this.strstart != 0)
        {
          this.pending[this.pendingCount++] = (byte) ((this._codec._Adler32 & 4278190080U) >> 24);
          this.pending[this.pendingCount++] = (byte) ((this._codec._Adler32 & 16711680U) >> 16);
          this.pending[this.pendingCount++] = (byte) ((this._codec._Adler32 & 65280U) >> 8);
          this.pending[this.pendingCount++] = (byte) (this._codec._Adler32 & (uint) byte.MaxValue);
        }
        this._codec._Adler32 = Adler.Adler32(0U, (byte[]) null, 0, 0);
      }
      if (this.pendingCount != 0)
      {
        this._codec.flush_pending();
        if (this._codec.AvailableBytesOut == 0)
        {
          this.last_flush = -1;
          return 0;
        }
      }
      else if (this._codec.AvailableBytesIn == 0 && flush <= (FlushType) lastFlush && flush != FlushType.Finish)
        return 0;
      if (this.status == DeflateManager.FINISH_STATE && this._codec.AvailableBytesIn != 0)
      {
        this._codec.Message = DeflateManager._ErrorMessage[7];
        throw new ZlibException("status == FINISH_STATE && _codec.AvailableBytesIn != 0");
      }
      if (this._codec.AvailableBytesIn != 0 || this.lookahead != 0 || flush != FlushType.None && this.status != DeflateManager.FINISH_STATE)
      {
        BlockState blockState = this.DeflateFunction(flush);
        if (blockState == BlockState.FinishStarted || blockState == BlockState.FinishDone)
          this.status = DeflateManager.FINISH_STATE;
        if (blockState == BlockState.NeedMore || blockState == BlockState.FinishStarted)
        {
          if (this._codec.AvailableBytesOut == 0)
            this.last_flush = -1;
          return 0;
        }
        if (blockState == BlockState.BlockDone)
        {
          if (flush == FlushType.Partial)
          {
            this._tr_align();
          }
          else
          {
            this._tr_stored_block(0, 0, false);
            if (flush == FlushType.Full)
            {
              for (int index = 0; index < this.hash_size; ++index)
                this.head[index] = (short) 0;
            }
          }
          this._codec.flush_pending();
          if (this._codec.AvailableBytesOut == 0)
          {
            this.last_flush = -1;
            return 0;
          }
        }
      }
      if (flush != FlushType.Finish)
        return 0;
      if (!this.WantRfc1950HeaderBytes || this.Rfc1950BytesEmitted)
        return 1;
      this.pending[this.pendingCount++] = (byte) ((this._codec._Adler32 & 4278190080U) >> 24);
      this.pending[this.pendingCount++] = (byte) ((this._codec._Adler32 & 16711680U) >> 16);
      this.pending[this.pendingCount++] = (byte) ((this._codec._Adler32 & 65280U) >> 8);
      this.pending[this.pendingCount++] = (byte) (this._codec._Adler32 & (uint) byte.MaxValue);
      this._codec.flush_pending();
      this.Rfc1950BytesEmitted = true;
      return this.pendingCount == 0 ? 1 : 0;
    }

    internal delegate BlockState CompressFunc(FlushType flush);

    internal class Config
    {
      internal int GoodLength;
      internal int MaxLazy;
      internal int NiceLength;
      internal int MaxChainLength;
      internal DeflateFlavor Flavor;
      private static readonly DeflateManager.Config[] Table = new DeflateManager.Config[10]
      {
        new DeflateManager.Config(0, 0, 0, 0, DeflateFlavor.Store),
        new DeflateManager.Config(4, 4, 8, 4, DeflateFlavor.Fast),
        new DeflateManager.Config(4, 5, 16, 8, DeflateFlavor.Fast),
        new DeflateManager.Config(4, 6, 32, 32, DeflateFlavor.Fast),
        new DeflateManager.Config(4, 4, 16, 16, DeflateFlavor.Slow),
        new DeflateManager.Config(8, 16, 32, 32, DeflateFlavor.Slow),
        new DeflateManager.Config(8, 16, 128, 128, DeflateFlavor.Slow),
        new DeflateManager.Config(8, 32, 128, 256, DeflateFlavor.Slow),
        new DeflateManager.Config(32, 128, 258, 1024, DeflateFlavor.Slow),
        new DeflateManager.Config(32, 258, 258, 4096, DeflateFlavor.Slow)
      };

      private Config(
        int goodLength,
        int maxLazy,
        int niceLength,
        int maxChainLength,
        DeflateFlavor flavor)
      {
        this.GoodLength = goodLength;
        this.MaxLazy = maxLazy;
        this.NiceLength = niceLength;
        this.MaxChainLength = maxChainLength;
        this.Flavor = flavor;
      }

      public static DeflateManager.Config Lookup(CompressionLevel level) => DeflateManager.Config.Table[(int) level];
    }
  }
}
