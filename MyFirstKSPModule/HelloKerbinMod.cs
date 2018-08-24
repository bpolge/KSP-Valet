using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace MyFirstKSPModule
{
    public class HelloKerbinMod : PartModule
    {
        public override void OnStart(StartState state)
        {
            Debug.LogError("MY_FIRST: OnStart");
        }
    }
}
