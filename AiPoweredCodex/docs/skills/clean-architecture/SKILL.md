---
name: clean-architecture
description: Design, review, or refactor applications around clean architecture boundaries. Use when Codex needs to organize code into domain, application, infrastructure, and presentation layers, reduce coupling, enforce dependency direction, split responsibilities, or migrate a tightly coupled codebase toward testable and maintainable modules.
---

# Clean Architecture

## Overview

Use this skill to shape code around business rules first and framework details second. Start from the current codebase, identify the real dependency flow, then move logic toward stable inner layers without rewriting everything at once.

## Workflow

1. Map the current modules, projects, and runtime entry points.
2. Identify domain rules, use cases, adapters, and framework-specific code.
3. Define the target boundaries before editing files.
4. Move logic incrementally so dependencies point inward.
5. Verify behavior with focused tests after each slice.

## Layer Rules

- Keep entities and core domain rules free of framework, database, UI, and transport concerns.
- Put use-case orchestration in the application layer.
- Implement interfaces, repositories, clients, and persistence details in the infrastructure layer.
- Keep controllers, API endpoints, UI handlers, and presenters thin.
- Depend on abstractions owned by inner layers, not on outer implementations.

## Refactoring Heuristics

- Extract business decisions out of controllers, ORM models, and service classes first.
- Introduce ports or interfaces at the seams where the application needs external data or side effects.
- Prefer small vertical slices over sweeping architecture rewrites.
- Preserve existing behavior; change structure before changing features.
- If a rule cannot be tested without a database or web server, it is probably in the wrong layer.

## Deliverables

- Explain the current structure and the target structure in plain terms.
- Name the files or modules that belong in each layer.
- Call out dependency violations explicitly.
- Make only the minimum edits needed to restore or improve the boundary.
- Read [references/layer-checklist.md](references/layer-checklist.md) when deciding where code belongs.

## Output Style

- State the architectural decision before editing.
- Reference concrete files and dependencies.
- Prefer explicit tradeoffs over generic best-practice language.
