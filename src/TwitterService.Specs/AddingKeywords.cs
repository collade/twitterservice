namespace TwitterService.Specs
{
    using TechTalk.SpecFlow;
    using NUnit.Framework;

    [Binding]
    public class AddingKeywords : SpecSetup
    {
        [Given(@"I am a user in organization ""(.*)""")]
        public void GivenIAmAUserInOrganization(string organization)
        {
            Assert.IsNotNull(_service);
            Assert.AreEqual(true, _service.AddOrganization(organization));
        }

        [When(@"I call AddKeyword function with parameters keyword ""(.*)"" and organization ""(.*)""")]
        public void WhenICallAddKeywordFunctionWithParametersKeywordAndOrganization(string keyword, string organization)
        {
            _service.AddKeyword(organization, keyword);
        }

        [Then(@"in db organization ""(.*)"" should have ""(.*)"" as saved")]
        public void ThenInDbOrganizationShouldHaveAsSaved(string organization, string keyword)
        {
            Assert.AreEqual(true, _service.HasKeywordForOrganization(organization, keyword));
        }
    }
}
