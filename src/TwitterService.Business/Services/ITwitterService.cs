namespace TwitterService.Business.Services {
    public interface ITwitterService {

        bool HasOrganization(string organizationId);
        bool HasKeywordForOrganization(string organizationId, string keyword);
        bool HasDistinctKeyword(string keyword);

        bool AddOrganization(string organizationId);
        bool AddKeyword(string organizationId, string keyword);

        bool RemoveKeyword(string organizationId, string keyword);
    }
}