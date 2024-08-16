﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DegCAD
{
    public interface ISvgConvertable
    {
        public string ToSvg();
        public bool IsVisible { get; }
    }
}