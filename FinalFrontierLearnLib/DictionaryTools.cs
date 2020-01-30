using System.Collections.Generic;
using System.IO;

namespace FinalFrontierLearnLib
{
    public class DictionaryTools
    {
        // https://www.dotnetperls.com/dictionary-binary
        public void Write(Dictionary<string, int> dictionary, string file)
        {
            if (dictionary.Count == 0)
                return;
            using (FileStream fs = File.OpenWrite(file))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                // Put count.
                writer.Write(dictionary.Count);
                // Write pairs.
                foreach (var pair in dictionary)
                {
                    writer.Write(pair.Key);
                    writer.Write(pair.Value);
                }
            }
        }

        public Dictionary<string, int> Read(string file)
        {
            var result = new Dictionary<string, int>();
            using (FileStream fs = File.OpenRead(file))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                // Get count.
                int count = reader.ReadInt32();
                // Read in all pairs.
                for (int i = 0; i < count; i++)
                {
                    string key = reader.ReadString();
                    int value = reader.ReadInt32();
                    result[key] = value;
                }
            }
            return result;
        }
    }
}
