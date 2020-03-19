using System.Collections.Generic;
using System.IO;

namespace FinalFrontierLearnLib
{
    // TODO: Catch opening errors 
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


        public void WriteHashSet(HashSet<string> hashSet, string file)
        {
            if (hashSet.Count == 0)
                return;
            using(FileStream fs = File.OpenWrite(file))
            using(BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(hashSet.Count);
                foreach (var hash in hashSet)
                {
                    writer.Write(hash);
                }
            }
        }

        public HashSet<string> ReadHasSet(string file)
        {
            if (!File.Exists(file))
                return new HashSet<string>();
            var result = new HashSet<string>();
            using (FileStream fs = File.OpenRead(file))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++)
                {
                    result.Add(reader.ReadString());
                }
            }
            return result;
        }
    }
}
