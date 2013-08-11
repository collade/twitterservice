namespace TwitterService.Specs
{
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class SearchInTwitter : SpecSetup
    {
        [When(@"I call Search function with parameters keyword ""(.*)""")]
        public void WhenICallSearchFunctionWithParametersKeyword(string keyword)
        {
            _service.Search(keyword);
        }

        [Then(@"if any there is any tweet db should have ""(.*)"" containing tweets saved")]
        public void ThenIfAnyThereIsAnyTweetDbShouldHaveContainingTweetsSaved(string keyword)
        {
            Assert.Greater(_service.Search(keyword), 0);
        }
    }
}
