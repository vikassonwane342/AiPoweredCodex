# GitHub Issue To PR Checklist

Use this sequence for issue-driven work.

## Inspect

- Check `git status --short --branch`.
- Check the default branch and remotes.
- Read the issue and identify acceptance criteria.

## Branch

- Switch to `main`.
- Pull the latest changes if permissions and network access allow it.
- Create a new branch from `main`.

## Implement

- Change only the files needed for the issue.
- Add or update tests when behavior changes.
- Run targeted validation before broader validation.

## Commit

- Stage only relevant files.
- Use a focused commit message such as `fix: handle null token refresh for issue #123`.

## Push And PR

- Push with upstream tracking.
- Open a PR targeting `main`.
- Include issue context, implementation summary, and verification notes.

## If Blocked

- Leave the code in a clean local state.
- Report whether the block is auth, network, remote permission, or missing tooling.
