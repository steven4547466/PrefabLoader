using CommandSystem;
using Exiled.API.Enums;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrefabLoader
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "Steven4547466";

        public override PluginPriority Priority => PluginPriority.First;
    }
}
