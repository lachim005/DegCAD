﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    /// <summary>
    /// Contains Monge items generated by one command
    /// </summary>
    public class TimelineItem : IEnumerable<ITimelineElement>
    {
        public ITimelineElement[] Items { get; set; }

        public TimelineItem(ITimelineElement[] items)
        {
            Items = items;
        }

        public ITimelineElement this[int index]
        {
            get => Items[index];
            set => Items[index] = value;
        }

        public IEnumerator<ITimelineElement> GetEnumerator() => Items.ToList().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

    }
}
