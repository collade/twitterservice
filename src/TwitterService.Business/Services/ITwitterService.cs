namespace TwitterService.Business.Services
{
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Threading.Tasks;

    using global::TwitterService.Business.Entities;

    [ServiceContract]
    public interface ITwitterService
    {
        [OperationContract]
        bool HasOrganization(string organizationId);
        [OperationContract]
        bool HasKeywordForOrganization(string organizationId, string keyword);
        [OperationContract]
        bool HasDistinctKeyword(string keyword);

        [OperationContract]
        bool AddOrganization(string organizationId);
        [OperationContract]
        bool AddKeyword(string organizationId, string keyword);

        [OperationContract]
        bool RemoveKeyword(string organizationId, string keyword);

        [OperationContract]
        int Search(string keyword);

        [OperationContract]
        bool Run();

        [OperationContract]
        List<string> GetKeywords(string organizationId);

        [OperationContract]
        Task<List<TweetDto>> GetTweetItems(string organizationId);
    }
}