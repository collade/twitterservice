Feature: Disabling Keywords
	In order to manage my keywords in twitter service
	As a user
	I want to disable a keyword
	So that service don't need to search for that keywords in twitter

Scenario: Disable a keyword
	Given I have entered the "keyword" and the "organizationId"	
	When I call DisableKeyword function
	Then the result should be true 
	And in db IsDisabled field should be true
