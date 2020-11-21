﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PoskusCiv2.Enums;

namespace PoskusCiv2.Improvements
{
    internal class Cathedral : BaseImprovement
    {
        public Cathedral() : base()
        {
            Type = ImprovementType.Cathedral;
            Name = "Cathedral";
        }
    }
}