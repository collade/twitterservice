namespace TwitterService.Business.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Threading.Tasks;

    using LinqToTwitter;
    using MongoDB.Driver.Builders;
    using Newtonsoft.Json;

    using global::TwitterService.Business.Entities;
    using global::TwitterService.Business.Repos;

    public class TwitterService : BaseService, ITwitterService
    {
        private readonly IEntityRepository<Organization> organizationRepository;
        private readonly IEntityRepository<Keyword> keywordRepository;
        private readonly IEntityRepository<DistinctKeyword> distinctKeywordRepository;
        private readonly IEntityRepository<Tweet> tweetRepository;

        public TwitterService(
            IEntityRepository<Organization> organizationRepository,
            IEntityRepository<Keyword> keywordRepository,
            IEntityRepository<DistinctKeyword> distinctKeywordRepository,
            IEntityRepository<Tweet> tweetRepository)
        {
            this.organizationRepository = organizationRepository;
            this.keywordRepository = keywordRepository;
            this.distinctKeywordRepository = distinctKeywordRepository;
            this.tweetRepository = tweetRepository;
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
                    const string OrganizationId = "1";

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
                    this.AddKeyword(OrganizationId, "collaborative project management tool");
                    this.AddKeyword(OrganizationId, "business chat");
                    this.AddKeyword(OrganizationId, "girl");
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
                                var _key = string.Empty;
                                foreach (var key in keys)
                                {
                                    if (text.ToLowerInvariant().Contains(key))
                                    {
                                        _key = key;
                                        break;
                                    }
                                }

                                this.tweetRepository.Add(
                                    new Tweet
                                    {
                                        CreatedBy = "System",
                                        UpdatedBy = "System",
                                        TweetText = text,
                                        TweetStatusID = obj.id_str,
                                        TwitterUserID = obj.user.id_str,
                                        TwitterUserImageUrl = obj.user.profile_image_url_https,
                                        TwitterUserName = obj.user.screen_name,
                                        CreatedAt = DateTime.ParseExact((string)obj.created_at, "ddd MMM dd HH:mm:ss zzz yyyy", CultureInfo.InvariantCulture),
                                        UpdatedAt = DateTime.Now,
                                        Keyword = _key
                                    });
                            }
                            catch (Exception)
                            {

                            }
                        }
                    }).SingleOrDefault();
        }
    }
}