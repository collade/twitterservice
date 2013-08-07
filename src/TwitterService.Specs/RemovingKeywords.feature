Feature: Removing Keywords
	In order to manage my keywords in twitter service
	As a user
	I want to remove a keyword
	So that service don't need to search for that keywords in twitter

Scenario: Disable a keyword
	Given I am a user in organization "1"
	When I call RemoveKeyword function with parameters keyword "keyword" and organization "1"
	Then in db organization "1" should not have "keyword" as saved