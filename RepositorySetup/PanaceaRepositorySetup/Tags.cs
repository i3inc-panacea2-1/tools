﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PanaceaRepositorySetup
{
    public class Tag
    {
        public Tag()
        {
            Tags = new List<Tag>();
        }

        public string Name { get; set; }

        public List<Tag> Tags { get; set; }
    }

   
}
