namespace TwitterService.Specs
{
    using NUnit.Framework;
    using TechTalk.SpecFlow;

    [Binding]
    public class GettingTweets : SpecSetup
    {
        [When(@"I call GetTweetItems function with parameter organization ""(.*)""")]
        public void WhenICallGetTweetItemsFunctionWithParameterOrganization(string organizationId)
        {
            _service.GetTweetItems(organizationId);
        }

        [Then(@"service shoud retun the tweets in a list")]
        public void ThenServiceShoudRetunTheTweetsInAList()
        {
            Assert.IsNotNull(_service.GetTweetItems(OrganizationId).Result);
        }
    }
}
