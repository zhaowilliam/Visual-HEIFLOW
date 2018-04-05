﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Core.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Running
{
    public enum SequenceType { Accumulative, StepbyStep }
    public  class MonitorItem : IMonitorItem
    {
        protected string _Name;
        protected MonitorItem _Parent;

        
        public MonitorItem(string name)
        {
            _Name = name;
            Group = "Custom";
            Derivable = false;
            Children = new List<MonitorItem>();
            this.SequenceType = SequenceType.StepbyStep;
        }

        public string Name
        {
            get
            {
                return _Name;
            }
        }

        public int VariableIndex
        {
            get;
            set;
        }

        public bool Derivable
        {
            get;
            set;
        }

        public int[] DerivedIndex
        {
            get;
            set;
        }

        public string Group
        {
            get;
            set;
        }

        public List<MonitorItem> Children
        {
            get;
            set;
        }
        public SequenceType SequenceType
        {
            get;
            set;
        }

        public IFileMonitor Monitor
        {
            get;
            set;
        }

        public MonitorItem Parent
        {
            get
            {
                return _Parent;
            }
            set
            {
                _Parent = value;
           
                if(!_Parent.Children.Contains(this))
                {
                    _Parent.Children.Add(this);
                }
            }
        }


        public double[] DerivedValues
        {
            get;
            set;
        }

        public virtual double [] Derive(ListTimeSeries<double> source)
        {
            if (DerivedIndex != null)
            {
                double[] values = new double[source.Dates.Count];
                for (int i = 0; i < source.Dates.Count; i++)
                {
                    foreach(var index in DerivedIndex)
                    {
                        values[i] += source.Values[index][i];
                    }
                }
                return values;
            }
            else
            {
                return null;
            }
        }

        public virtual DataTable ToDataTable(ListTimeSeries<double> sourcedata)
        {
            DataTable dt = new DataTable();
            DataColumn date_col = new DataColumn("Date", Type.GetType("System.DateTime"));
            dt.Columns.Add(date_col);
            DataColumn value_col = new DataColumn(this.Name, Type.GetType("System.Double"));
            dt.Columns.Add(value_col);
            double[] buf = null;
            if (Derivable)
            {
                if (this.DerivedValues == null)
                    this.DerivedValues = this.Derive(sourcedata);
                buf = this.DerivedValues;
            }
            else
            {
                buf = sourcedata.Values[this.VariableIndex].ToArray();
            }
            for (int i = 0; i < buf.Length; i++)
            {
                var dr = dt.NewRow();
                dr[0] = sourcedata.Dates[i];
                dr[1] = buf[i];
                dt.Rows.Add(dr);
            }
            return dt;
        }


    }
}
