﻿//
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Heiflow.Core
{
    [Serializable]
    public class Units:IUnits
    {
        public Units()
        {
        }

        public Units(int id)
        {
            mId = id;
            Name = "Unknown";
            AliasName = "Unknown";
            UnitType = "Unknown";
            Abbreviation = "Unknown";

        }
        private int mId;

        #region IUnits 成员

        public int ID
        {
            get
            {
                return mId;
            }
        }

        public string Name
        {
            get;
            set;
        }

        public string AliasName
        {
            get;
            set;
        }

        public string UnitType
        {
            get;
            set;
        }

        public string Abbreviation
        {
            get;
            set;
        }

        #endregion
    }
}
