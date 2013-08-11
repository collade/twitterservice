Feature: Realtime Search Saved Keywords
	In order to realtime search saved keywords in twitter
	As a user
	I want to search a keyword in public stream
	So that service will save tweets containing keywords

Scenario: Search tweet stream for saved keyword	
	When the run method called
	Then if there is any tweet containing saved keywords shoul be saved to db