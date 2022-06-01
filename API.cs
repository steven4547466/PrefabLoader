using Exiled.API.Features;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.RepresentationModel;

namespace PrefabLoader
{
    public static class API
    {
        public static Prefab LoadPrefab(string file)
        {
            return LoadPrefab(new StringReader(File.ReadAllText(file)));
        }

        public static Prefab LoadPrefab(StringReader reader)
        {
            YamlStream stream = new YamlStream();
            stream.Load(reader);
            return new Prefab(stream);
        }

        public static void LoadMaterial(string file)
        {
            string text = File.ReadAllText(file);
            
            // idk how to read yml tags directly, deal with it.
            string id = text.Substring(52, 10).Split('\n')[0].Trim();
            LoadMaterial(new StringReader(text), id);
        }
        
        public static void LoadMaterial(StringReader reader, string id)
        {
            YamlStream stream = new YamlStream();
            stream.Load(reader);
            new Material(stream, id);
        }
    }
}
