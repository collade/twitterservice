namespace TwitterService.Specs
{
    using System.Configuration;

    using MongoDB.Driver;

    using PusherServer;

    using TwitterService.Business.Entities;
    using TwitterService.Business.Repos;
    using TwitterService.Business.Services;

    public class SpecSetup
    {
        public ITwitterService _service;

        public IEntityRepository<Keyword> _keywordRepository;
        public IEntityRepository<DistinctKeyword> _distinctKeywordRepository;
        public IEntityRepository<Organization> _organizationRepository;
        public IEntityRepository<Tweet> _tweetRepository;
        public Pusher _pusherServer;

        public SpecSetup()
        {
            _keywordRepository = new EntityRepository<Keyword>();
            _distinctKeywordRepository = new EntityRepository<DistinctKeyword>();
            _organizationRepository = new EntityRepository<Organization>();
            _tweetRepository = new EntityRepository<Tweet>();
            _pusherServer = new Pusher(ConfigurationManager.AppSettings["pusherAppId"], ConfigurationManager.AppSettings["pusherAppKey"], ConfigurationManager.AppSettings["pusherAppSecret"]);


            _service = new TwitterService(_organizationRepository, _keywordRepository, _distinctKeywordRepository, _tweetRepository, _pusherServer);

            var mongoCnnStr = ConfigurationManager.AppSettings["MongoCnnStr"];
            var mongoDatabase = new MongoClient(mongoCnnStr).GetServer().GetDatabase(ConfigurationManager.AppSettings["MongoDBName"]);
            mongoDatabase.Drop();
        }

        public string OrganizationId = "51fb4622902c7f0fecca0343";
    }
}