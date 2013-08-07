using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace TwitterService.Specs
{
    using NUnit.Framework;

    using TwitterService.Business.Entities;
    using TwitterService.Business.Repos;
    using TwitterService.Business.Services;

    [Binding]
    public class AddingKeywords
    {
        TwitterService _service;
        IEntityRepository<Keyword> _keywordRepository;
        IEntityRepository<DistinctKeyword> _distinctKeywordRepository;
        IEntityRepository<Organization> _organizationRepository;

        [Given(@"I am a user in organization ""(.*)""")]
        public void GivenIAmAUserInOrganization(string organization)
        {
            _keywordRepository = new EntityRepository<Keyword>();
            _distinctKeywordRepository = new EntityRepository<DistinctKeyword>();
            _organizationRepository = new EntityRepository<Organization>();

            _service = new TwitterService(_organizationRepository, _keywordRepository, _distinctKeywordRepository);

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
