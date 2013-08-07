namespace TwitterService.Business.Services
{
    using System.Linq;

    using global::TwitterService.Business.Entities;
    using global::TwitterService.Business.Repos;

    public class TwitterService : BaseService
    {
        private readonly IEntityRepository<Organization> organizationRepository;
        private readonly IEntityRepository<Keyword> keywordRepository;
        private readonly IEntityRepository<DistinctKeyword> distinctKeywordRepository;

        public TwitterService(
            IEntityRepository<Organization> organizationRepository,
            IEntityRepository<Keyword> keywordRepository,
            IEntityRepository<DistinctKeyword> distinctKeywordRepository)
        {

            this.organizationRepository = organizationRepository;
            this.keywordRepository = keywordRepository;
            this.distinctKeywordRepository = distinctKeywordRepository;
        }

        public bool HasOrganization(string organizationId)
        {
            return organizationRepository.AsQueryable().Any(x => x.OrganizationId == organizationId);
        }

        public bool AddOrganization(string organizationId)
        {
            if (string.IsNullOrEmpty(organizationId))
            {
                return false;
            }

            if (!HasOrganization(organizationId))
            {
                return false;
            }

            var result =
                organizationRepository.Add(
                    new Organization { CreatedBy = "System", UpdatedBy = "System", OrganizationId = organizationId });

            return result.Ok;
        }

        public bool HasKeywordForOrganization(string organizationId, string keyword)
        {
            return keywordRepository.AsQueryable().Any(x => x.OrganizationId == organizationId && x.Key == keyword);
        }

        public bool HasDistinctKeyword(string keyword)
        {
            return distinctKeywordRepository.AsQueryable().Any(x => x.Key == keyword);
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
    }
}