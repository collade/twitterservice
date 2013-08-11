using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace TwitterService.Specs
{
    [Binding]
    public class SearchInTwitter: SpecSetup
    {
        [When(@"I call Search function with parameters keyword ""(.*)""")]
        public void WhenICallSearchFunctionWithParametersKeyword(string keyword) {
            _service.Search(keyword);
        }

        [Then(@"if any there is any tweet db should have ""(.*)"" containing tweets saved")]
        public void ThenIfAnyThereIsAnyTweetDbShouldHaveContainingTweetsSaved(string keyword)
        {
            
        }
    }
}
