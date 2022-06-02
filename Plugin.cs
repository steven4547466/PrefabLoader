using Exiled.API.Enums;
using Exiled.API.Features;

namespace PrefabLoader
{
    public class Plugin : Plugin<Config>
    {
        public override string Author => "Steven4547466";

        public override PluginPriority Priority => PluginPriority.First;

        public override void OnEnabled()
        {
            Exiled.Events.Handlers.Server.RestartingRound += EventHandlers.OnRestartingRound;

            Exiled.Events.Handlers.Player.Verified += EventHandlers.OnVerified;

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Exiled.Events.Handlers.Server.RestartingRound -= EventHandlers.OnRestartingRound;

            Exiled.Events.Handlers.Player.Verified -= EventHandlers.OnVerified;

            base.OnDisabled();
        }
    }
}
