using System.IO;
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

        public static void LoadMaterial(string materialFile, string metaDataFile)
        {
            YamlStream stream = new YamlStream();
            stream.Load(new StringReader(File.ReadAllText(metaDataFile)));

            string id = ((YamlMappingNode)stream.Documents[0].RootNode).Children[Prefab.ScalarNodes["guid"]].ToString();

            LoadMaterial(new StringReader(File.ReadAllText(materialFile)), id);
        }

        public static void LoadMaterial(StringReader reader, string id)
        {
            YamlStream stream = new YamlStream();
            stream.Load(reader);
            new Material(stream, id);
        }
    }
}
