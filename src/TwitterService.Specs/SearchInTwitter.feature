Feature: Search In Twitter
	In order to search keywords in twitter
	As a user
	I want to search a keyword in public latest tweets
	So that service will save tweets containing keyword

Scenario: Search a keyword	
	When I call Search function with parameters keyword "keyword"
	Then if any there is any tweet db should have "keyword" containing tweets saved