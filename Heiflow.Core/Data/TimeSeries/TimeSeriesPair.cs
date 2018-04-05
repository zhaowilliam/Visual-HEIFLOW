﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    public class TimeSeriesPair<T>
    {
        public TimeSeriesPair(DateTime date, T value)
        {
            DateTime = date;
            Value = value;
        }

        public DateTime DateTime
        {
            get;
            set;
        }

        public T Value
        {
            get;
            set;
        }
    }
}
