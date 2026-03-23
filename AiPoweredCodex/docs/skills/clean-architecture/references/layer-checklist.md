# Layer Checklist

Use this checklist when placing or reviewing code.

## Domain

- Entities, value objects, domain services, and business invariants live here.
- Keep this layer free of HTTP, SQL, ORM, messaging, UI, and framework APIs.

## Application

- Use cases coordinate domain objects and ports.
- Accept commands or queries, invoke domain logic, and return simple results.
- Depend only on the domain layer and abstractions for external systems.

## Infrastructure

- Implement repositories, gateways, event publishers, token providers, and database access.
- Translate between external formats and application/domain models.

## Presentation

- Accept HTTP, CLI, UI, or message inputs.
- Validate request shape, call application services, and map outputs to transport responses.

## Smells

- Controller contains business branching.
- Entity imports ORM or web framework types.
- Use case directly imports a database client.
- Infrastructure decides business policy.
