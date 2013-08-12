namespace TwitterService.Business.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    using LinqToTwitter;
    using MongoDB.Driver.Builders;
    using Newtonsoft.Json;

    using PusherServer;

    using global::TwitterService.Business.Entities;
    using global::TwitterService.Business.Repos;

    public class TwitterService : BaseService, ITwitterService
    {
        private readonly IEntityRepository<Organization> organizationRepository;
        private readonly IEntityRepository<Keyword> keywordRepository;
        private readonly IEntityRepository<DistinctKeyword> distinctKeywordRepository;
        private readonly IEntityRepository<Tweet> tweetRepository;
        private readonly Pusher pusherServer;

        public TwitterService(
            IEntityRepository<Organization> organizationRepository,
            IEntityRepository<Keyword> keywordRepository,
            IEntityRepository<DistinctKeyword> distinctKeywordRepository,
            IEntityRepository<Tweet> tweetRepository,
            Pusher pusherServer)
        {
            this.organizationRepository = organizationRepository;
            this.keywordRepository = keywordRepository;
            this.distinctKeywordRepository = distinctKeywordRepository;
            this.tweetRepository = tweetRepository;
            this.pusherServer = pusherServer;
        }

        public bool HasOrganization(string organizationId)
        {
            return organizationRepository.AsQueryable().Any(x => x.OrganizationId == organizationId);
        }

        public bool HasKeywordForOrganization(string organizationId, string keyword)
        {
            return keywordRepository.AsQueryable().Any(x => x.OrganizationId == organizationId && x.Key == keyword);
        }

        public bool HasDistinctKeyword(string keyword)
        {
            return distinctKeywordRepository.AsQueryable().Any(x => x.Key == keyword);
        }

        public bool AddOrganization(string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId))
            {
                return false;
            }

            if (HasOrganization(organizationId))
            {
                return false;
            }

            var result =
                organizationRepository.Add(
                    new Organization { CreatedBy = "System", UpdatedBy = "System", OrganizationId = organizationId });

            return result.Ok;
        }

        public bool AddKeyword(string organizationId, string keyword)
        {
            if (string.IsNullOrEmpty(organizationId) ||
                string.IsNullOrEmpty(keyword))
            {
                return false;
            }

            if (!HasOrganization(organizationId))
            {
                return false;
            }

            if (HasKeywordForOrganization(organizationId, keyword))
            {
                return false;
            }

            keyword = keyword.ToLowerInvariant();

            var result =
                keywordRepository.Add(
                    new Keyword
                    {
                        CreatedBy = "System",
                        UpdatedBy = "System",
                        OrganizationId = organizationId,
                        Key = keyword
                    });

            if (result.Ok)
            {
                if (!HasDistinctKeyword(keyword))
                {
                    var result2 =
                        distinctKeywordRepository.Add(
                            new DistinctKeyword { CreatedBy = "System", UpdatedBy = "System", Key = keyword });

                    var twitterContext = GetTwitterContext();
                    this.RealTimeSearchTwitter(twitterContext, keyword, new List<string> { keyword });

                    if (!result2.Ok)
                    {
                        // just trying again but will need to somehing better...
                        distinctKeywordRepository.Add(
                           new DistinctKeyword { CreatedBy = "System", UpdatedBy = "System", Key = keyword });
                    }

                    return true;
                }

                return true;
            }

            return false;
        }

        public bool RemoveKeyword(string organizationId, string keyword)
        {
            if (string.IsNullOrEmpty(organizationId) ||
                string.IsNullOrEmpty(keyword))
            {
                return false;
            }

            if (!HasOrganization(organizationId))
            {
                return false;
            }

            if (!HasKeywordForOrganization(organizationId, keyword))
            {
                return false;
            }

            var result =
                keywordRepository.Update(
                    Query.And(
                        Query<Keyword>.EQ(x => x.Key, keyword),
                        Query<Keyword>.EQ(x => x.OrganizationId, organizationId)),
                    Update<Keyword>.Set(x => x.IsDeleted, true)
                                   .Set(x => x.DeletedAt, DateTime.Now)
                                   .Set(x => x.DeletedBy, "System"));

            if (result.Ok)
            {
                if (!keywordRepository.AsQueryable().Any(x => x.Key == keyword))
                {
                    var result2 =
                        distinctKeywordRepository.Update(
                            Query.And(Query<DistinctKeyword>.EQ(x => x.Key, keyword)),
                            Update<DistinctKeyword>.Set(x => x.IsDeleted, true)
                                                   .Set(x => x.DeletedAt, DateTime.Now)
                                                   .Set(x => x.DeletedBy, "System"));

                    if (!result2.Ok)
                    {
                        //another something better needed place...
                        distinctKeywordRepository.Update(
                            Query.And(Query<DistinctKeyword>.EQ(x => x.Key, keyword)),
                            Update<DistinctKeyword>.Set(x => x.IsDeleted, true)
                                                   .Set(x => x.DeletedAt, DateTime.Now)
                                                   .Set(x => x.DeletedBy, "System"));
                    }
                }

                return true;
            }

            return false;
        }

        public int Search(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
            {
                return 0;
            }

            var twitterContext = GetTwitterContext();

            var items = twitterContext.Search.Single(x => x.Type == SearchType.Search && x.Query == keyword);
            foreach (var item in items.Statuses)
            {
                tweetRepository.Add(new Tweet
                {
                    CreatedBy = "System",
                    UpdatedBy = "System",

                    TweetText = item.Text,
                    TweetStatusID = item.StatusID,

                    TwitterUserID = item.User.Identifier.UserID,
                    TwitterUserImageUrl = item.User.ProfileImageUrlHttps,
                    TwitterUserName = item.User.Identifier.ScreenName,

                    CreatedAt = item.CreatedAt,
                    UpdatedAt = DateTime.Now,
                    Keyword = keyword
                });
            }

            return items.Statuses.Count;
        }

        public bool Run()
        {
            try
            {
                if (!distinctKeywordRepository.AsQueryable().Any())
                {
                    const string OrganizationId = "51fb4622902c7f0fecca0343";

                    this.AddOrganization(OrganizationId);

                    this.AddKeyword(OrganizationId, "collade");
                    this.AddKeyword(OrganizationId, "agile");
                    this.AddKeyword(OrganizationId, "scrum");
                    this.AddKeyword(OrganizationId, "kanban");
                    this.AddKeyword(OrganizationId, "startup");
                    this.AddKeyword(OrganizationId, "tech startup");
                    this.AddKeyword(OrganizationId, "collaboration");
                    this.AddKeyword(OrganizationId, "project management");
                    this.AddKeyword(OrganizationId, "task management");
                    this.AddKeyword(OrganizationId, "team management");
                    this.AddKeyword(OrganizationId, "collaboration software");
                    this.AddKeyword(OrganizationId, "collaborative project management");
                    this.AddKeyword(OrganizationId, "collaborative project management tool");
                    this.AddKeyword(OrganizationId, "collaborative task management");
                    this.AddKeyword(OrganizationId, "business chat");
                    this.AddKeyword(OrganizationId, "group chat");

                    this.AddKeyword(OrganizationId, "flowdock");
                    this.AddKeyword(OrganizationId, "trello");
                    this.AddKeyword(OrganizationId, "kanbanize");
                    this.AddKeyword(OrganizationId, "pivotal tracker");
                    this.AddKeyword(OrganizationId, "kanbanery");
                }

                var keywords = distinctKeywordRepository.AsQueryable().ToList().Select(x => x.Key).ToList();

                var keys = string.Empty;
                foreach (var key in keywords)
                {
                    keys += string.Format("{0},", key);
                }

                var twitterContext = GetTwitterContext();
                this.RealTimeSearchTwitter(twitterContext, keys, keywords);

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static TwitterContext GetTwitterContext()
        {
            var auth = new SingleUserAuthorizer
            {
                Credentials =
                    new SingleUserInMemoryCredentials
                    {
                        ConsumerKey =
                            "onZSPzzGF2TbmcRFenWg",
                        ConsumerSecret =
                            "06tif8gnaOnuCo0nziIoPETw0xpf2Ve2tgBOzQ",
                        TwitterAccessToken =
                            "18249700-hLL3tsmnE5yNVBxpjt080k4fimhC1R4YdRFFoLAuQ",
                        TwitterAccessTokenSecret =
                            "M2BXS5XBxD3ta5YknXjAeoZ44qZSJnXFLVKzEsdWlY"
                    }
            };
            return new TwitterContext(auth);
        }


        private void RealTimeSearchTwitter(TwitterContext twitterContext, string keyword, List<string> keys)
        {
            var streamItems =
                twitterContext.Streaming.Where(x => x.Type == StreamingType.Filter && x.Track == keyword).StreamingCallback(
                    x =>
                    {
                        if (x.Status == TwitterErrorStatus.Success)
                        {
                            try
                            {
                                dynamic obj = JsonConvert.DeserializeObject(x.Content);

                                string text = obj.text;
                                var tag = string.Empty;
                                foreach (var key in keys)
                                {
                                    if (text.ToLowerInvariant().Contains(key))
                                    {
                                        tag = key;
                                        break;
                                    }
                                }

                                if (!string.IsNullOrEmpty(tag))
                                {
                                    string userName = obj.user.screen_name;
                                    string userImgUrl = obj.user.profile_image_url_https;
                                    string statusId = obj.id_str;
                                    var time = DateTime.ParseExact((string)obj.created_at, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture);
                                    string time2 = time.ToString("dd MMMM dddd - HH:mm", CultureInfo.InvariantCulture);

                                    //find who you wil notify
                                    var keywords = keywordRepository.AsQueryable().Where(y => y.Key == tag);
                                    foreach (var organizationKeyword in keywords)
                                    {
                                        var orgKey = organizationKeyword;
                                        ThreadPool.QueueUserWorkItem(m => pusherServer.Trigger(string.Format("presence-{0}", orgKey.OrganizationId), "tweet_added",
                                            new { statusId, text, tag, userName, userImgUrl, time2 }));
                                    }

                                    this.tweetRepository.Add(
                                        new Tweet
                                        {
                                            CreatedBy = "System",
                                            UpdatedBy = "System",
                                            TweetText = text,
                                            TweetStatusID = statusId,
                                            TwitterUserID = obj.user.id_str,
                                            TwitterUserImageUrl = userImgUrl,
                                            TwitterUserName = userName,
                                            CreatedAt = time,
                                            UpdatedAt = DateTime.Now,
                                            Keyword = tag
                                        });
                                }
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }).SingleOrDefault();
        }
    }
}