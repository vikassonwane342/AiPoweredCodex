---
name: ai-sdlc-cycle
description: Execute an issue-driven AI SDLC workflow for GitHub repositories. Use when Codex needs to take a GitHub issue assigned to the current user, fetch the issue title and description, understand the acceptance criteria, create a branch from main, implement the fix, validate the change, push the branch, and open a pull request back to main.
---

# Ai Sdlc Cycle

## Overview

Use this skill when the work starts from a GitHub issue assigned to you. The skill should retrieve the assigned issue details first, then drive the full implementation cycle from analysis to PR without losing traceability to the issue.

## Workflow

1. Identify the repository, issue number, and the current GitHub user.
2. Retrieve the assigned issue title, description, labels, and comments from GitHub.
3. Restate the bug or feature request and extract clear acceptance criteria.
4. Inspect the local repository state before changing branches.
5. Switch to `main` and pull the latest changes when permissions allow it.
6. Create a new branch from `main` named from the issue number and short slug.
7. Implement the code changes needed to satisfy the issue.
8. Add or update tests for the changed behavior.
9. Run the relevant validation commands.
10. Commit only the issue-related files.
11. Push the branch to origin.
12. Open a pull request targeting `main` and reference the issue.

## GitHub Retrieval Rules

- Prefer `gh issue view <number>` when GitHub CLI is available and authenticated.
- If the issue number is not provided, use `gh issue list --assignee @me` to find issues assigned to the current user.
- Read the issue body before coding.
- Read recent comments when they affect scope or acceptance criteria.
- If GitHub access is blocked, stop and report the exact blocked command or missing authentication.

## Branching Rules

- Always branch from `main` unless the repository clearly uses a different default branch.
- Prefer branch names like `issue-456-fix-order-total`.
- Do not mix unrelated local changes into the issue branch.
- Do not overwrite or revert user work that is unrelated to the issue.

## Implementation Rules

- Map the issue description to concrete code locations before editing.
- Keep the change set small and directly tied to the assigned issue.
- Update tests whenever behavior changes or a bug is fixed.
- Verify both the success path and the failure path when the issue involves validation, auth, or data flow.
- Read [references/assigned-issue-flow.md](references/assigned-issue-flow.md) for the exact command sequence.

## Pull Request Rules

- Target `main`.
- Include the issue number in the PR title or body when repo conventions expect it.
- Summarize the root cause, the implemented fix, and the validation performed.
- Mention skipped tests, blockers, or follow-up work explicitly.

## Output Style

- State which issue was selected or retrieved.
- State the branch name, validation result, and PR target.
- If any step could not be completed, report the exact failure and the last successful step.
