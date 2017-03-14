﻿using System.Collections.Generic;
using Persistity.Attributes;

namespace Assets.Tests.Editor
{
    [Persist]
    public class A
    {
        [PersistData]
        public string TestValue { get; set; }

        [PersistData]
        public B NestedValue { get; set; }

        [PersistData]
        public B[] NestedArray { get; set; }
        
        [PersistData]
        public IList<string> Stuff { get; set; }

        public A()
        {
            Stuff = new List<string>();
        }
    }
}
