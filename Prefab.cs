using Exiled.API.Features;
using Exiled.API.Features.Toys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace PrefabLoader
{
    public class Prefab
    {
        public List<PrimitiveData> PrimitiveData = new List<PrimitiveData>();
        
        public Prefab(YamlStream yaml)
        {
            Log.Info("Loading");
            Vector3 nextPosition = Vector3.zero;
            Quaternion nextRotation = default;
            Vector3 nextScale = Vector3.zero;
            string nextTransformId = null;
            string nextParentTransformId = null;
            Color nextColor = Color.white;
            PrimitiveType? nextType = null;

            for (int i = 0; i < yaml.Documents.Count; i++)
            {                
                var mapping = (YamlMappingNode)yaml.Documents[i].RootNode;
                if (mapping.Children.TryGetValue(new YamlScalarNode("GameObject"), out var go))
                {
                    if (nextType != null)
                    {
                        Log.Info(nextColor);
                        PrimitiveData data = new PrimitiveData((PrimitiveType)nextType, nextPosition, nextRotation, nextScale, nextTransformId, nextColor);
                        PrimitiveData.Add(data);
                        data.SetParent(nextParentTransformId, PrimitiveData);

                        nextParentTransformId = null;
                        nextType = null;
                        nextColor = Color.white;
                    }
                    
                    YamlMappingNode gameObject = (YamlMappingNode)go;
                    var components = gameObject.Children[new YamlScalarNode("m_Component")];
                    YamlMappingNode transform = (YamlMappingNode)components[0];
                    YamlMappingNode comp = (YamlMappingNode) transform.Children[new YamlScalarNode("component")];
                    nextTransformId = comp.Children[new YamlScalarNode("fileID")].ToString();
                }
                else if (mapping.Children.TryGetValue(new YamlScalarNode("Transform"), out var t))
                {
                    YamlMappingNode transform = (YamlMappingNode)t;
                    var position = (YamlMappingNode)transform.Children[new YamlScalarNode("m_LocalPosition")];
                    var rotation = (YamlMappingNode)transform.Children[new YamlScalarNode("m_LocalRotation")];
                    var scale = (YamlMappingNode)transform.Children[new YamlScalarNode("m_LocalScale")];
                    nextPosition = new Vector3(float.Parse(position.Children[new YamlScalarNode("x")].ToString()), float.Parse(position.Children[new YamlScalarNode("y")].ToString()), float.Parse(position.Children[new YamlScalarNode("z")].ToString()));
                    nextRotation = new Quaternion(float.Parse(rotation.Children[new YamlScalarNode("x")].ToString()), float.Parse(rotation.Children[new YamlScalarNode("y")].ToString()), float.Parse(rotation.Children[new YamlScalarNode("z")].ToString()), float.Parse(rotation.Children[new YamlScalarNode("w")].ToString()));
                    nextScale = new Vector3(float.Parse(scale.Children[new YamlScalarNode("x")].ToString()), float.Parse(scale.Children[new YamlScalarNode("y")].ToString()), float.Parse(scale.Children[new YamlScalarNode("z")].ToString()));
                    
                    var parent = (YamlMappingNode)transform.Children[new YamlScalarNode("m_Father")];

                    nextParentTransformId = parent.Children[new YamlScalarNode("fileID")].ToString();
                }
                else if (mapping.Children.TryGetValue(new YamlScalarNode("MeshFilter"), out var mf))
                {
                    YamlMappingNode meshFilter = (YamlMappingNode)mf;

                    YamlMappingNode mesh = (YamlMappingNode)meshFilter.Children[new YamlScalarNode("m_Mesh")];

                    int identifier = int.Parse(mesh.Children[new YamlScalarNode("fileID")].ToString());
                    
                    switch(identifier)
                    {
                        case 10202:
                            nextType = PrimitiveType.Cube;
                            break;
                        case 10206:
                            nextType = PrimitiveType.Cylinder;
                            break;
                        case 10207:
                            nextType = PrimitiveType.Sphere;
                            break;
                        case 10208:
                            nextType = PrimitiveType.Capsule;
                            break;
                        case 10209:
                            nextType = PrimitiveType.Plane;
                            break;
                        case 10210:
                            nextType = PrimitiveType.Quad;
                            break;
                        default:
                            nextType = null;
                            break;
                    }
                } 
                else if (mapping.Children.TryGetValue(new YamlScalarNode("MeshRenderer"), out var mr))
                {
                    YamlMappingNode meshRenderer = (YamlMappingNode)mr;

                    YamlMappingNode materials = (YamlMappingNode)meshRenderer.Children[new YamlScalarNode("m_Materials")][0];

                    nextColor = Material.GetColor(materials.Children[new YamlScalarNode("fileID")].ToString());
                }
            }

            if (nextType != null)
            {
                PrimitiveData data = new PrimitiveData((PrimitiveType)nextType, nextPosition, nextRotation, nextScale, nextTransformId, nextColor);
                PrimitiveData.Add(data);
                data.SetParent(nextParentTransformId, PrimitiveData);
            }
        }
        
        public SpawnedPrefab Spawn()
        {
            return new SpawnedPrefab(new GameObject(), PrimitiveData);
        }

        public SpawnedPrefab Spawn(Vector3 position)
        {
            SpawnedPrefab prefab = Spawn();
            prefab.Origin.transform.position = position;
            return prefab;
        }
    }

    public class SpawnedPrefab
    {
        public GameObject Origin { get; private set; }
        private List<Primitive> Primitives = new List<Primitive>();

        public SpawnedPrefab(GameObject gameObject, List<PrimitiveData> primitives)
        {
            Origin = gameObject;

            Dictionary<string, Primitive> primitiveMap = new Dictionary<string, Primitive>();

            foreach (PrimitiveData primitiveData in primitives)
            {
                Primitive p = Primitive.Create();

                p.Position = primitiveData.Position;
                p.Type = primitiveData.PrimitiveType;
                p.Color = primitiveData.Color;
                p.Scale = primitiveData.Scale;
                p.Rotation = primitiveData.Rotation;

                if (primitiveData.ParentTransformId != null)
                {
                    if (primitiveMap.TryGetValue(primitiveData.ParentTransformId, out Primitive parent))
                        p.Base.transform.SetParent(parent.Base.transform, false);
                    else
                    {
                        Log.Error("Attempted to spawn child before parent...");
                    }
                }
                else
                {
                    p.Base.transform.SetParent(Origin.transform, false);
                }

                primitiveMap.Add(primitiveData.TransformId, p);
                Primitives.Add(p);
            }
        }

        public void SetPosition(Vector3 position)
        {
            Origin.transform.position = position;
        }

        public void SetRotation(Quaternion rotation)
        {
            Origin.transform.rotation = rotation;
        }
    }

    public class PrimitiveData
    {
        public PrimitiveType PrimitiveType { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public string TransformId { get; set; }
        public Color Color { get; set; }

        public string ParentTransformId { get; set; } = null;

        public PrimitiveData(PrimitiveType primitiveType, Vector3 position, Quaternion rotation, Vector3 scale, string transformId, Color color)
        {
            PrimitiveType = primitiveType;
            Position = position;
            Rotation = rotation;
            Scale = scale;
            TransformId = transformId;
            Color = color;
        }

        public void SetParent(string parentTransformId, List<PrimitiveData> primitiveData)
        {
            if (parentTransformId == "0")
                return;
            
            ParentTransformId = parentTransformId;
        }
    }
}
