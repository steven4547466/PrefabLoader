using Exiled.API.Interfaces;

namespace PrefabLoader
{
    public sealed class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
    }
}
