using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.RepresentationModel;

namespace PrefabLoader
{
    public class Material
    {
        internal static Dictionary<string, Material> Materials { get; } = new Dictionary<string, Material>();

        public string Id { get; set; }
        public Color Color { get; set; }

        public Material(YamlStream yaml, string id)
        {
            Id = id;
            YamlDocument document = yaml.Documents[0];

            var mapping = (YamlMappingNode)document.RootNode;

            if (mapping.Children.TryGetValue(new YamlScalarNode("Material"), out var mat))
            {
                YamlMappingNode material = (YamlMappingNode)mat;

                YamlMappingNode props = (YamlMappingNode)material.Children[new YamlScalarNode("m_SavedProperties")];

                YamlMappingNode colors = (YamlMappingNode)((YamlMappingNode)props.Children[new YamlScalarNode("m_Colors")][0]).Children[new YamlScalarNode("_Color")];

                Color = new Color(float.Parse(colors.Children[new YamlScalarNode("r")].ToString()), float.Parse(colors.Children[new YamlScalarNode("g")].ToString()), float.Parse(colors.Children[new YamlScalarNode("b")].ToString()), float.Parse(colors.Children[new YamlScalarNode("a")].ToString()));
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
