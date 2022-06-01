using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace PrefabLoader
{
    public class Material
    {
        internal static Dictionary<string, Material> Materials { get; } = new Dictionary<string, Material>();

        internal static Dictionary<string, YamlScalarNode> ScalarNodes = new Dictionary<string, YamlScalarNode>()
        {
            { "Material", new YamlScalarNode("Material") },
            { "m_SavedProperties", new YamlScalarNode("m_SavedProperties") },
            { "m_Colors", new YamlScalarNode("m_Colors") },
            { "_Color", new YamlScalarNode("_Color") },
            { "r", new YamlScalarNode("r") },
            { "g", new YamlScalarNode("g") },
            { "b", new YamlScalarNode("b") },
            { "a", new YamlScalarNode("a") },
        };

        public string Id { get; set; }
        public Color Color { get; set; }

        public Material(YamlStream yaml, string id)
        {
            Id = id;
            YamlDocument document = yaml.Documents[0];

            var mapping = (YamlMappingNode)document.RootNode;

            if (mapping.Children.TryGetValue(ScalarNodes["Material"], out var mat))
            {
                YamlMappingNode material = (YamlMappingNode)mat;

                YamlMappingNode props = (YamlMappingNode)material.Children[ScalarNodes["m_SavedProperties"]];

                YamlMappingNode colors = (YamlMappingNode)((YamlMappingNode)props.Children[ScalarNodes["m_Colors"]][0]).Children[ScalarNodes["_Color"]];

                Color = new Color(float.Parse(colors.Children[ScalarNodes["r"]].ToString()), float.Parse(colors.Children[ScalarNodes["g"]].ToString()), float.Parse(colors.Children[ScalarNodes["b"]].ToString()), float.Parse(colors.Children[ScalarNodes["a"]].ToString()));
            }

            Materials.Add(Id, this);
        }

        public static Color GetColor(string id)
        {
            if (Materials.TryGetValue(id, out Material material))
            {
                return material.Color;
            }
            else
            {
                return Color.white;
            }
        }
    }
}
