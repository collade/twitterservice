namespace TwitterService.Business.Entities
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class TweetDto
    {
        [DataMember]
        public string Link { get; set; }

        [DataMember]
        public string UserImageUrl { get; set; }
        [DataMember]
        public string UserName { get; set; }

        [DataMember]
        public string StatusID { get; set; }
        [DataMember]
        public string Text { get; set; }
        [DataMember]
        public string Tag { get; set; }
        [DataMember]
        public string Time { get; set; }

        [DataMember]
        public DateTime CreatedAt { get; set; }
    }
}