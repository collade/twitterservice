Feature: Getting Keywords
	In order to manage my keywords in twitter service
	As a user
	I want to get keywords by organizationId 
	So that service will return all keywords of organization

Scenario: Get organizations keywords
	Given I am a user in organization "1"	
	When I call GetKeywords function with parameter organization "1"
	Then service shoud retun the keywrods in a list

