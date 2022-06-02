using AdminToys;
using Exiled.API.Features;
using Exiled.API.Features.Toys;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace PrefabLoader
{
    /// <summary>
    /// A prefab that has not yet been spawned.
    /// </summary>
    public class Prefab
    {
        internal static Dictionary<string, YamlScalarNode> ScalarNodes = new Dictionary<string, YamlScalarNode>()
        {
            { "GameObject", new YamlScalarNode("GameObject") },
            { "m_Component", new YamlScalarNode("m_Component") },
            { "component", new YamlScalarNode("component") },
            { "fileID", new YamlScalarNode("fileID") },
            { "Transform", new YamlScalarNode("Transform") },
            { "m_LocalPosition", new YamlScalarNode("m_LocalPosition") },
            { "m_LocalRotation", new YamlScalarNode("m_LocalRotation") },
            { "m_LocalScale", new YamlScalarNode("m_LocalScale") },
            { "x", new YamlScalarNode("x") },
            { "y", new YamlScalarNode("y") },
            { "z", new YamlScalarNode("z") },
            { "w", new YamlScalarNode("w") },
            { "m_Father", new YamlScalarNode("m_Father") },
            { "MeshFilter", new YamlScalarNode("MeshFilter") },
            { "m_Mesh", new YamlScalarNode("m_Mesh") },
            { "MeshRenderer", new YamlScalarNode("MeshRenderer") },
            { "m_Materials", new YamlScalarNode("m_Materials") },
            { "guid", new YamlScalarNode("guid") },
        };

        public List<PrimitiveData> PrimitiveData = new List<PrimitiveData>();

        public Prefab(YamlStream yaml)
        {
            Vector3 nextPosition = Vector3.zero;
            Quaternion nextRotation = default;
            Vector3 nextScale = Vector3.zero;
            string nextTransformId = null;
            string nextParentTransformId = null;
            Color nextColor = Color.white;
            bool nextIsEmpty = true;
            PrimitiveType nextType = PrimitiveType.Cube;

            for (int i = 0; i < yaml.Documents.Count; i++)
            {
                var mapping = (YamlMappingNode)yaml.Documents[i].RootNode;
                if (mapping.Children.TryGetValue(ScalarNodes["GameObject"], out var go))
                {
                    if (i > 0)
                    {
                        PrimitiveData data = new PrimitiveData(nextType, nextPosition, nextRotation, nextScale, nextTransformId, nextColor, nextIsEmpty);
                        PrimitiveData.Add(data);
                        data.SetParent(nextParentTransformId, PrimitiveData);
                        nextParentTransformId = null;
                        nextColor = Color.white;
                        nextIsEmpty = true;
                    }

                    YamlMappingNode gameObject = (YamlMappingNode)go;
                    var components = gameObject.Children[ScalarNodes["m_Component"]];
                    YamlMappingNode transform = (YamlMappingNode)components[0];
                    YamlMappingNode comp = (YamlMappingNode)transform.Children[ScalarNodes["component"]];
                    nextTransformId = comp.Children[ScalarNodes["fileID"]].ToString();
                }
                else if (mapping.Children.TryGetValue(ScalarNodes["Transform"], out var t))
                {
                    YamlMappingNode transform = (YamlMappingNode)t;
                    var position = (YamlMappingNode)transform.Children[ScalarNodes["m_LocalPosition"]];
                    var rotation = (YamlMappingNode)transform.Children[ScalarNodes["m_LocalRotation"]];
                    var scale = (YamlMappingNode)transform.Children[ScalarNodes["m_LocalScale"]];
                    nextPosition = new Vector3(float.Parse(position.Children[ScalarNodes["x"]].ToString()), float.Parse(position.Children[ScalarNodes["y"]].ToString()), float.Parse(position.Children[ScalarNodes["z"]].ToString()));
                    nextRotation = new Quaternion(float.Parse(rotation.Children[ScalarNodes["x"]].ToString()), float.Parse(rotation.Children[ScalarNodes["y"]].ToString()), float.Parse(rotation.Children[ScalarNodes["z"]].ToString()), float.Parse(rotation.Children[ScalarNodes["w"]].ToString()));
                    nextScale = new Vector3(float.Parse(scale.Children[ScalarNodes["x"]].ToString()), float.Parse(scale.Children[ScalarNodes["y"]].ToString()), float.Parse(scale.Children[ScalarNodes["z"]].ToString()));

                    var parent = (YamlMappingNode)transform.Children[ScalarNodes["m_Father"]];

                    nextParentTransformId = parent.Children[ScalarNodes["fileID"]].ToString();
                }
                else if (mapping.Children.TryGetValue(ScalarNodes["MeshFilter"], out var mf))
                {
                    YamlMappingNode meshFilter = (YamlMappingNode)mf;

                    YamlMappingNode mesh = (YamlMappingNode)meshFilter.Children[ScalarNodes["m_Mesh"]];

                    int identifier = int.Parse(mesh.Children[ScalarNodes["fileID"]].ToString());

                    switch (identifier)
                    {
                        case 10202:
                            nextIsEmpty = false;
                            nextType = PrimitiveType.Cube;
                            break;
                        case 10206:
                            nextIsEmpty = false;
                            nextType = PrimitiveType.Cylinder;
                            break;
                        case 10207:
                            nextIsEmpty = false;
                            nextType = PrimitiveType.Sphere;
                            break;
                        case 10208:
                            nextIsEmpty = false;
                            nextType = PrimitiveType.Capsule;
                            break;
                        case 10209:
                            nextIsEmpty = false;
                            nextType = PrimitiveType.Plane;
                            break;
                        case 10210:
                            nextIsEmpty = false;
                            nextType = PrimitiveType.Quad;
                            break;
                        default:
                            Log.Error("Unknown primitive type: " + identifier);
                            nextType = PrimitiveType.Cube;
                            break;
                    }
                }
                else if (mapping.Children.TryGetValue(ScalarNodes["MeshRenderer"], out var mr))
                {
                    YamlMappingNode meshRenderer = (YamlMappingNode)mr;

                    YamlMappingNode materials = (YamlMappingNode)meshRenderer.Children[ScalarNodes["m_Materials"]][0];

                    nextColor = Material.GetColor(materials.Children[ScalarNodes["guid"]].ToString());
                }
            }


            {  // Cry about it, if this isn't here, I can't use data in the loop
                PrimitiveData data = new PrimitiveData(nextType, nextPosition, nextRotation, nextScale, nextTransformId, nextColor, nextIsEmpty);
                PrimitiveData.Add(data);
                data.SetParent(nextParentTransformId, PrimitiveData);
            }
        }

        /// <summary>
        /// Spawns the prefab onto the clients.
        /// </summary>
        /// <param name="position">The position to spawn it at.</param>
        /// <param name="rotation">The rotation to spawn it with.</param>
        /// <param name="players">The players to spawn it on (broken).</param>
        /// <returns>The <see cref="SpawnedPrefab"/></returns>
        public SpawnedPrefab Spawn(Vector3 position = default, Quaternion rotation = default, List<Player> players = null)
        {
            // Currently can't support spawning on specific players?
            // I actually don't know why this doesn't work. It also causes some major desync and lag.
            SpawnedPrefab prefab = new SpawnedPrefab(new GameObject(), PrimitiveData, true, players);
            prefab.Origin.transform.position = position;
            prefab.Origin.transform.rotation = rotation;

            return prefab;
        }
    }

    /// <summary>
    /// A prefab that has been spawned.
    /// </summary>
    public class SpawnedPrefab
    {
        internal static List<SpawnedPrefab> SpawnedPrefabs = new List<SpawnedPrefab>();

        public GameObject Origin { get; private set; }

        internal List<Player> AllowedPlayers = null;
        internal List<PrimitiveObjectToy> Primitives = new List<PrimitiveObjectToy>();

        public SpawnedPrefab(GameObject gameObject, List<PrimitiveData> primitives, bool spawn = true, List<Player> players = null)
        {
            Origin = gameObject;

            AllowedPlayers = players;

            Dictionary<string, Transform> parentMap = new Dictionary<string, Transform>();

            foreach (PrimitiveData primitiveData in primitives)
            {
                if (primitiveData.IsEmpty)
                {
                    GameObject empty = new GameObject();
                    empty.transform.position = primitiveData.Position;
                    empty.transform.localScale = primitiveData.Scale;
                    empty.transform.rotation = primitiveData.Rotation;

                    if (primitiveData.ParentTransformId != null)
                    {
                        if (parentMap.TryGetValue(primitiveData.ParentTransformId, out Transform parent))
                            empty.transform.SetParent(parent, false);
                        else
                        {
                            Log.Error("Attempted to spawn child before parent...");
                        }
                    }
                    else
                    {
                        empty.transform.SetParent(Origin.transform, false);
                    }

                    parentMap.Add(primitiveData.TransformId, empty.transform);
                }
                else
                {
                    PrimitiveObjectToy p = Object.Instantiate(ToysHelper.PrimitiveBaseObject);

                    p.transform.position = primitiveData.Position;
                    p.NetworkPrimitiveType = primitiveData.PrimitiveType;
                    p.NetworkMaterialColor = primitiveData.Color;
                    p.transform.localScale = primitiveData.Scale;
                    p.transform.rotation = primitiveData.Rotation;

                    if (primitiveData.ParentTransformId != null)
                    {
                        if (parentMap.TryGetValue(primitiveData.ParentTransformId, out Transform parent))
                            p.transform.SetParent(parent, false);
                        else
                        {
                            Log.Error("Attempted to spawn child before parent...");
                        }
                    }
                    else
                    {
                        p.transform.SetParent(Origin.transform, false);
                    }

                    if (spawn)
                    {
                        NetworkServer.Spawn(p.gameObject);
                        if (players != null)
                        {
                            foreach (Player player in Player.List)
                            {
                                if (!players.Contains(player))
                                    player.Connection.Send(new ObjectDestroyMessage() { netId = p.netId });
                            }
                        }
                    }

                    parentMap.Add(primitiveData.TransformId, p.transform);
                    Primitives.Add(p);
                }
            }

            SpawnedPrefabs.Add(this);
        }

        /// <summary>
        /// Sets the position of the origin and translates the entire prefab to the location.
        /// </summary>
        /// <param name="position">The position to place the object.</param>
        public void SetPosition(Vector3 position)
        {
            Origin.transform.position = position;
        }

        /// <summary>
        /// Sets the rotation of the origin and rotates the entire prefab to the rotation.
        /// </summary>
        /// <param name="rotation">The rotation of the object.</param>
        public void SetRotation(Quaternion rotation)
        {
            Origin.transform.rotation = rotation;
        }

        /// <summary>
        /// Destroys a spawned prefab.
        /// </summary>
        public void Destroy()
        {
            foreach (PrimitiveObjectToy p in Primitives)
            {
                NetworkServer.Destroy(p.gameObject);
            }

            Primitives = null;
            AllowedPlayers = null;
            Origin = null;

            SpawnedPrefabs.Remove(this);
        }
    }

    /// <summary>
    /// Holds data necessary to spawn a primitive.
    /// </summary>
    public class PrimitiveData
    {
        public PrimitiveType PrimitiveType { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public string TransformId { get; set; }
        public Color Color { get; set; }

        public bool IsEmpty { get; set; }

        public string ParentTransformId { get; set; } = null;

        public PrimitiveData(PrimitiveType primitiveType, Vector3 position, Quaternion rotation, Vector3 scale, string transformId, Color color, bool isEmpty = false)
        {
            PrimitiveType = primitiveType;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            TransformId = transformId;
            Color = color;
            IsEmpty = isEmpty;
        }

        public void SetParent(string parentTransformId, List<PrimitiveData> primitiveData)
        {
            if (parentTransformId == "0")
                return;

            ParentTransformId = parentTransformId;
        }
    }
}
