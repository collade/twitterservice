namespace TwitterService.Business.Entities {
    public class Tweet :BaseEntity {
        public string Keyword { get; set; }
        

        public string TwitterUserID { get; set; }
        public string TwitterUserImageUrl { get; set; }
        public string TwitterUserName { get; set; }

        public string TweetStatusID { get; set; }
        public string TweetText { get; set; }
    }
}