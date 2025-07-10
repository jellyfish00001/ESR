using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace ERS
{
    public class JsonHelper
    {
        public static List<T> Read<T>(string path)
        {
            JsonSerializer serializer = new JsonSerializer();
            using (var file = new StreamReader(Path.Combine(Directory.GetCurrentDirectory(), path)))
            using (var reader = new JsonTextReader(file))
            {
                return (List<T>)serializer.Deserialize(reader, typeof(List<T>));
            }
        }
    }
}
