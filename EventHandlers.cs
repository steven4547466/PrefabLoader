using AdminToys;
using Exiled.Events.EventArgs;
using Mirror;

namespace PrefabLoader
{
    public static class EventHandlers
    {
        public static void OnVerified(VerifiedEventArgs ev)
        {
            foreach (SpawnedPrefab spawnedPrefab in SpawnedPrefab.SpawnedPrefabs)
            {
                if (spawnedPrefab.AllowedPlayers != null)
                {
                    foreach (PrimitiveObjectToy primitive in spawnedPrefab.Primitives)
                        ev.Player.Connection.Send(new ObjectDestroyMessage() { netId = primitive.netId });
                }
            }
        }

        public static void OnRestartingRound()
        {
            foreach (SpawnedPrefab spawnedPrefab in SpawnedPrefab.SpawnedPrefabs)
            {
                spawnedPrefab.Destroy();
            }

            SpawnedPrefab.SpawnedPrefabs.Clear();
        }
    }
}
