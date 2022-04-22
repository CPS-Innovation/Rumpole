using Newtonsoft.Json;

namespace RumpoleGateway.Wrappers
{
    public class JsonConvertWrapper : IJsonConvertWrapper
    {
        public string SerializeObject(object objectToSerialize)
        {
            return JsonConvert.SerializeObject(objectToSerialize, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
        }

        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
