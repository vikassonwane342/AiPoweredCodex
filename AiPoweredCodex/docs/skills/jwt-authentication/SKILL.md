---
name: jwt-authentication
description: Implement, review, or troubleshoot JWT-based authentication and authorization flows. Use when Codex needs to add login endpoints, issue access or refresh tokens, validate bearer tokens, secure APIs, model claims and roles, handle token rotation, or fix broken authentication middleware in web services and APIs.
---

# Jwt Authentication

## Overview

Use this skill to build JWT flows that are correct first, convenient second. Treat token issuance, storage, validation, expiry, and authorization as separate concerns and keep secrets and signing configuration out of source code.

## Workflow

1. Identify the auth entry points, protected routes, and current user model.
2. Decide whether the app needs access tokens only or access plus refresh tokens.
3. Define claims, issuer, audience, expiry, and signing algorithm before editing.
4. Implement token creation and validation in one place.
5. Add authorization checks after authentication works reliably.
6. Verify failure paths, expiry handling, and revoked-token behavior.

## Security Rules

- Keep signing keys and secrets in environment-specific configuration, never inline.
- Use short-lived access tokens.
- Store refresh tokens separately and rotate them on refresh when the system supports sessions.
- Validate signature, expiry, issuer, audience, and token type.
- Reject unsigned or weakly signed tokens unless the system explicitly requires them and the risk is accepted.

## Implementation Rules

- Keep credential verification separate from JWT generation.
- Centralize JWT configuration and clock-skew handling.
- Return consistent unauthorized and forbidden responses.
- Put role or permission checks near application policies, not scattered across controllers.
- Read [references/jwt-checklist.md](references/jwt-checklist.md) when choosing claims and token handling patterns.

## Common Fixes

- Add missing authentication middleware to protected routes.
- Fix mismatched issuer, audience, or secret settings across environments.
- Separate access-token expiry from refresh-token expiry.
- Remove sensitive data from token payloads.
- Introduce refresh-token persistence if logout and revocation are required.

## Output Style

- Call out the token lifecycle explicitly.
- Explain where secrets, claims, and middleware live in the codebase.
- Note any security tradeoff instead of silently accepting it.
