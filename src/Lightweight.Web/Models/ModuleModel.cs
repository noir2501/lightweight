﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lightweight.Web.Models
{
    public class ModuleModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Configuration { get; set; }
    }
}