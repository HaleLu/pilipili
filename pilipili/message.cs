using System.Runtime.Serialization;

namespace pilipili
{
    [DataContract]
    public class Message
    {
        [DataMember]
        public int id { get; set; }

        [DataMember]
        public string name { get; set; }

        [DataMember]
        public string content { get; set; }

        [DataMember]
        public string time { get; set; }
    }
}