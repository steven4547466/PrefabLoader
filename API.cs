using System.IO;
using YamlDotNet.RepresentationModel;

namespace PrefabLoader
{
    public static class API
    {
        /// <summary>
        /// Loads a prefab from a file.
        /// </summary>
        /// <param name="file">The file location to get the data from.</param>
        /// <returns>An unspawned <see cref="Prefab"/>.</returns>
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

        /// <summary>
        /// Loads a material from a file.
        /// </summary>
        /// <param name="materialFile">The file location to get the material data from.</param>
        /// <param name="metaDataFile">The material's metadata file location.</param>
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
