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
        public static Prefab thing;

        public override string Author => "Steven4547466";

        public override PluginPriority Priority => PluginPriority.First;

        public override void OnEnabled()
        {
            API.LoadMaterial("D:\\Unity Projects\\Project A\\Project B\\Assets\\New Material.mat", "D:\\Unity Projects\\Project A\\Project B\\Assets\\New Material.mat.meta");
            thing = API.LoadPrefab("D:\\Unity Projects\\Project A\\Project B\\Assets\\Scenes\\Cube (1).prefab");
            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            base.OnDisabled();
        }
    }
}
