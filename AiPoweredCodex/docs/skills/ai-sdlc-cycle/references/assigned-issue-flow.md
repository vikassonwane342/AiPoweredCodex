# Assigned Issue Flow

Use this sequence for issue-assigned work.

## Discover The Issue

- Run `gh auth status` to confirm GitHub authentication.
- If the issue number is known, run `gh issue view <number> --comments`.
- If the issue number is not known, run `gh issue list --assignee @me` and select the relevant issue.
- Capture the issue title, description, labels, and recent comments.

## Prepare The Branch

- Run `git status --short --branch`.
- Confirm the default branch, usually `main`.
- Switch to `main`.
- Pull the latest changes if remote access is available.
- Create a new branch from `main`.

## Implement The Fix

- Translate issue requirements into file-level changes.
- Add or update tests.
- Run targeted validation first, then broader validation if needed.

## Commit And Push

- Stage only relevant files.
- Use a focused commit message such as `fix: resolve issue #456 order total rounding`.
- Push with upstream tracking.

## Raise The PR

- Open a PR targeting `main`.
- Reference the issue in the PR body.
- Include implementation summary and test evidence.

## If Blocked

- Report whether the blocker is GitHub auth, network access, missing `gh`, missing remote permission, or failing tests.
- Leave the local branch in a usable state.
