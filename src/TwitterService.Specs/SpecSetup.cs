namespace TwitterService.Specs {
    using System.Configuration;

    using MongoDB.Driver;

    using TwitterService.Business.Entities;
    using TwitterService.Business.Repos;
    using TwitterService.Business.Services;

    public class SpecSetup {
        public ITwitterService _service;

        public IEntityRepository<Keyword> _keywordRepository;
        public IEntityRepository<DistinctKeyword> _distinctKeywordRepository;
        public IEntityRepository<Organization> _organizationRepository;

        public SpecSetup() {
            _keywordRepository = new EntityRepository<Keyword>();
            _distinctKeywordRepository = new EntityRepository<DistinctKeyword>();
            _organizationRepository = new EntityRepository<Organization>();

            _service = new TwitterService(_organizationRepository, _keywordRepository, _distinctKeywordRepository);

            var mongoCnnStr = ConfigurationManager.AppSettings["MongoCnnStr"];
            var mongoDatabase = new MongoClient(mongoCnnStr).GetServer().GetDatabase(ConfigurationManager.AppSettings["MongoDBName"]);
            mongoDatabase.Drop();
        }
    }
}