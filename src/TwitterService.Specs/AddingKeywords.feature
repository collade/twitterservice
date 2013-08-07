Feature: Adding Keywords
	In order to manage my keywords in twitter service
	As a user
	I want to add a keyword
	So that service will search for that keyword in twitter

Scenario: Add a keyword
	Given I am a user in organization "1"	
	When I call AddKeyword function with parameters keyword "keyword" and organization "1"
	Then in db organization "1" should have "keyword" as saved

