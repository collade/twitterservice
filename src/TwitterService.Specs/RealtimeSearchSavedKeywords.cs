namespace TwitterService.Specs
{
    using NUnit.Framework;
    using TechTalk.SpecFlow;

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
            Assert.AreEqual(true, _service.Run());
        }
    }
}
