using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace TwitterService.Specs
{
    using NUnit.Framework;

    [Binding]
    public class GettingKeywords : SpecSetup
    {

        [When(@"I call GetKeywords function with parameter organization ""(.*)""")]
        public void WhenICallGetKeywordsFunctionWithParameterOrganization(string organizationId)
        {
            _service.GetKeywords(organizationId);
        }

        [Then(@"service shoud retun the keywrods in a list")]
        public void ThenServiceShoudRetunTheKeywrodsInAList()
        {
            Assert.IsNotNull(_service.GetKeywords(OrganizationId));
        }
    }
}
