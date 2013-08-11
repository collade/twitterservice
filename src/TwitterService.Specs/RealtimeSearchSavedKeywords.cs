using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;

namespace TwitterService.Specs
{
    [Binding]
    public class RealtimeSearchSavedKeywords : SpecSetup
    {
        [When(@"the run method called")]
        public void WhenTheRunMethodCalled()
        {
            _service.Run();
        }

        [Then(@"if there is any tweet containing saved keywords shoul be saved to db")]
        public void ThenIfThereIsAnyTweetContainingSavedKeywordsShoulBeSavedToDb()
        {

            ScenarioContext.Current.Pending();
        }
    }
}
