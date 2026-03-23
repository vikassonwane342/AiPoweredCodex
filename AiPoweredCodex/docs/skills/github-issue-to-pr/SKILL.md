---
name: github-issue-to-pr
description: Resolve a GitHub issue end to end by creating a branch from main, implementing the fix, validating the change, pushing the branch, and opening a pull request back to main. Use when Codex is asked to work from an issue URL or issue number and complete the standard GitHub development workflow with branch naming, commits, push, and PR creation.
---

# Github Issue To Pr

## Overview

Use this skill to turn an issue into a reviewable pull request with minimal manual cleanup. Keep the branch scoped to one issue, preserve unrelated local changes, and do not open a PR until the code and validation are in a credible state.

## Workflow

1. Read the issue carefully and restate the expected behavior.
2. Inspect the current branch and local working tree before changing anything.
3. Update `main` if the environment and permissions allow it.
4. Create a new branch from `main` using the issue number and a short slug.
5. Implement the fix and add or update tests.
6. Run the relevant validation commands.
7. Commit only the issue-related changes.
8. Push the branch to origin.
9. Open a PR targeting `main` with a concise summary, testing notes, and issue link.

## Branch And Commit Rules

- Base the work on `main` unless the repository uses a different default branch and that is verified locally.
- Prefer branch names like `issue-123-fix-login-timeout`.
- Keep commits focused and descriptive.
- Never revert unrelated user changes in a dirty worktree.

## Pull Request Rules

- Reference the issue in the PR body and commit messages when the repo convention expects it.
- Summarize user-visible behavior changes and the verification performed.
- Mention any remaining risks, skipped tests, or follow-up work.
- Read [references/github-flow.md](references/github-flow.md) before using Git or GitHub commands.

## Tooling Assumptions

- Prefer local git inspection first.
- Use `gh` CLI or repository-integrated tooling for issue and PR operations when available.
- If network access, auth, or permissions are missing, complete the code changes locally and report the exact blocked step.

## Output Style

- State the issue, branch name, validation status, and PR target explicitly.
- Report blockers with the exact command or permission that failed.
