using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;
public class JSONHelper
{
    public static string Serialize<T>(T obj)
    {
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
        MemoryStream ms = new MemoryStream();
        serializer.WriteObject(ms, obj);
        string retVal = Encoding.Default.GetString(ms.ToArray());
        return retVal;
    }
    public static T Deserialize<T>(string json)
    {
        MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
        DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
        T obj = (T)serializer.ReadObject(ms);
        ms.Close();
        return obj;
    }
}