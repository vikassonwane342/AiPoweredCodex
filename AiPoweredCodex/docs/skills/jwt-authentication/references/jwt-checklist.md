# JWT Checklist

Use this checklist before shipping JWT auth changes.

## Claims

- Include only identifiers and authorization data the server actually needs.
- Prefer `sub` for user identity.
- Add `iss`, `aud`, `iat`, and `exp` where supported by the framework.

## Access Tokens

- Keep lifetime short.
- Treat them as bearer credentials.
- Do not store passwords, secrets, or full profile payloads in the token.

## Refresh Tokens

- Persist them if the system needs logout, revocation, or device/session management.
- Rotate them on refresh when possible.
- Invalidate old refresh tokens after use.

## Middleware

- Authenticate first.
- Authorize second.
- Return `401` for missing or invalid identity and `403` for authenticated users lacking permission.

## Test Cases

- Valid token reaches protected endpoint.
- Expired token is rejected.
- Tampered token is rejected.
- Wrong issuer or audience is rejected.
- Revoked refresh token cannot mint a new access token.
