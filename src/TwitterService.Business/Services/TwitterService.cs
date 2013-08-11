namespace TwitterService.Business.Services
{
    using System;
    using System.Linq;
    using System.Threading;

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

            var auth = new SingleUserAuthorizer
            {
                Credentials =
                    new SingleUserInMemoryCredentials
                    {
                        ConsumerKey =
                            "UQxc0w8jgGdbyJlSnXyQ",
                        ConsumerSecret =
                            "tfqH0hb9zLuM4RNX1VwvKlovPsfHyCo5V0pBBwH5w",
                        TwitterAccessToken =
                            "18249700-hLL3tsmnE5yNVBxpjt080k4fimhC1R4YdRFFoLAuQ",
                        TwitterAccessTokenSecret =
                            "M2BXS5XBxD3ta5YknXjAeoZ44qZSJnXFLVKzEsdWlY"
                    }
            };

            var twitterContext = new TwitterContext(auth);

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

        public void Run() {
            var auth = new SingleUserAuthorizer
            {
                Credentials =
                    new SingleUserInMemoryCredentials
                    {
                        ConsumerKey =
                            "UQxc0w8jgGdbyJlSnXyQ",
                        ConsumerSecret =
                            "tfqH0hb9zLuM4RNX1VwvKlovPsfHyCo5V0pBBwH5w",
                        TwitterAccessToken =
                            "18249700-hLL3tsmnE5yNVBxpjt080k4fimhC1R4YdRFFoLAuQ",
                        TwitterAccessTokenSecret =
                            "M2BXS5XBxD3ta5YknXjAeoZ44qZSJnXFLVKzEsdWlY"
                    }
            };

            var twitterContext = new TwitterContext(auth);

            var streamItems = twitterContext.Streaming.Where(x => x.Type == StreamingType.Filter && x.Track == "girl").StreamingCallback(
                x =>
                {
                    if (x.Status == TwitterErrorStatus.Success)
                    {
                        dynamic obj = JsonConvert.DeserializeObject(x.Content);
                        tweetRepository.Add(new Tweet
                        {
                            CreatedBy = "System",
                            UpdatedBy = "System",

                            TweetText = obj.text,
                            TweetStatusID = obj.id_str,

                            TwitterUserID = obj.user.id_str,
                            TwitterUserImageUrl = obj.user.profile_image_url_https,
                            TwitterUserName = obj.user.screen_name,

                            CreatedAt = obj.created_at,
                            UpdatedAt = DateTime.Now,
                            Keyword = "girl"
                        });
                    }
                }).SingleOrDefault();
        }
    }
}