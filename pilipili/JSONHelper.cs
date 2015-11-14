using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;

public class JSONHelper
{
    public static string Serialize<T>(T obj)
    {
        var serializer = new DataContractJsonSerializer(obj.GetType());
        var ms = new MemoryStream();
        serializer.WriteObject(ms, obj);
        var retVal = Encoding.Default.GetString(ms.ToArray());
        return retVal;
    }

    public static T Deserialize<T>(string json)
    {
        var ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
        var serializer = new DataContractJsonSerializer(typeof (T));
        var obj = (T) serializer.ReadObject(ms);
        ms.Close();
        return obj;
    }
}