Feature: Getting Tweets
	In order to display tweets in the client
	As a user
	I want to get tweets saved by organizationId 
	So that service will return all tweets of organization

Scenario: Get organizations Twets
	Given I am a user in organization "1"	
	When I call GetTweetItems function with parameter organization "1"
	Then service shoud retun the tweets in a list

