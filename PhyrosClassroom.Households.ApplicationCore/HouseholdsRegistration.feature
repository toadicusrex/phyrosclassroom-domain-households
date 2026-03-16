Feature: Households registration

Scenario: Registering a household makes the read model available
	Given the Households application composition is configured
	When I register a household named "Alice" "Bennett"
	Then the registered household can be retrieved from the query side
