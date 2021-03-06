//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

namespace Excel.Core.BinaryFormat
{
	/// <summary>
	/// Represents row record in table
	/// </summary>
	internal class XlsBiffRow : XlsBiffRecord
	{
		internal XlsBiffRow(byte[] bytes, uint offset, ExcelBinaryReader reader)
			: base(bytes, offset, reader)
		{
		}

		/// <summary>
		/// Zero-based index of row described
		/// </summary>
		public ushort RowIndex
		{
			get { return base.ReadUInt16(0x0); }
		}

		/// <summary>
		/// Index of first defined column
		/// </summary>
		public ushort FirstDefinedColumn
		{
			get { return base.ReadUInt16(0x2); }
		}

		/// <summary>
		/// Index of last defined column
		/// </summary>
		public ushort LastDefinedColumn
		{
			get { return base.ReadUInt16(0x4); }
		}

		/// <summary>
		/// Returns row height
		/// </summary>
		public uint RowHeight
		{
			get { return base.ReadUInt16(0x6); }
		}

		/// <summary>
		/// Returns row flags
		/// </summary>
		public ushort Flags
		{
			get { return base.ReadUInt16(0xC); }
		}

		/// <summary>
		/// Returns default format for this row
		/// </summary>
		public ushort XFormat
		{
			get { return base.ReadUInt16(0xE); }
		}
	}
}