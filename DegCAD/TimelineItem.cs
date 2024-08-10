﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    /// <summary>
    /// Contains Monge items generated by one command
    /// </summary>
    public class TimelineItem
    {
        public ITimelineElement[] Items { get; set; }

        public TimelineItem(ITimelineElement[] items)
        {
            Items = items;
        }
    }
}
