namespace TwitterService.Business.Services {
    public interface ITwitterService {

        void SearchKeywordsInTweets();
        void AddSearchKeyword(string keyword);
        void DisableSearchKeyword(string keyword);
    }
}