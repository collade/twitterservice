using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace TwitterService.Specs
{
    using NUnit.Framework;

    [Binding]
    public class RemovingKeywords : SpecSetup
    {
        [When(@"I call RemoveKeyword function with parameters keyword ""(.*)"" and organization ""(.*)""")]
        public void WhenICallRemoveKeywordFunctionWithParametersKeywordAndOrganization(string keyword, string organization)
        {
            _service.RemoveKeyword(organization, keyword);
        }

        [Then(@"in db organization ""(.*)"" should not have ""(.*)"" as saved")]
        public void ThenInDbOrganizationShouldNotHaveAsSaved(string organization, string keyword)
        {
            Assert.AreEqual(false, _service.HasKeywordForOrganization(organization, keyword));
        }

    }
}
