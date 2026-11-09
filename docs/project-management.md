# Project management

How work on SDF Manipulator is tracked: issues in this repository and the
[SDF manipulator Kanban](https://github.com/users/T3sT3ro/projects/5) project. The setup follows
[github-project-template](https://github.com/T3sT3ro/github-project-template). Issues about the process itself carry
the `topic: meta` label.

## Model

Each piece of metadata has one job, so nothing is expressed twice.

| Concept              | Where it lives                   | Values                                                                  |
| -------------------- | -------------------------------- | ----------------------------------------------------------------------- |
| What kind of work    | `type: ...` labels (see below)   | epic > feature > story > task, plus bug                                 |
| Where it is in flow  | Project **Status**               | Backlog, Ready to work on, In progress, In review, Post-merge, Done     |
| How urgent           | Project **Priority**             | critical, high, normal (default), low                                   |
| How valuable         | Project **Impact**               | high, above-avg, below-avg, low                                         |
| How big              | Project **Size**                 | XS ~1 day, S ~3 days, M 1-2 weeks, L ~1 month, XL 1-2 months            |
| Which goal it serves | parent **epic** (sub-issues)     | see [Epics and milestones](#epics-and-milestones)                       |
| When                 | **Iteration**, **Milestone**     | Iteration = 14 days starting Monday; milestone = a release              |
| Why it is stuck      | `status: ...` labels             | needs-triage, needs-clarification, on-hold, cannot-reproduce, ...       |
| What area it touches | `topic: ...` labels              | AST, rendering, Unity integration, packaging, UX, DX, API, docs, ...    |
| Refinement of type   | `type: ...` labels               | experimental, refactor, enhancement                                     |
| Resolution           | `duplicate`, `invalid`, `wontfix` |                                                                        |

GitHub issue types only exist for organizations, and this repository belongs to a user. The `type: epic`,
`type: feature`, `type: story`, `type: task` and `type: bug` labels stand in for them. Use exactly one of them per
issue, and use sub-issues for the hierarchy: an epic holds features, a feature holds stories, a story holds tasks.

Priority, Impact and Size are project fields, not labels. Don't add priority or size labels.

## Epics and milestones

Epics group work by goal. An epic's description says what the goal is and when the epic is done. Attach an issue to
the epic whose goal it serves, directly as a sub-issue or through a feature or story under that epic. An issue has at
most one parent, so pick the epic that it matters most for. Small standalone tasks, such as CI fixes, don't need an
epic. Epics have Priority, Impact and Size like any other issue, so they can be ranked against each other on the board.

Milestones are releases of the package (`0.1`, `0.2`, ...). Give an issue the milestone of the release that it should
ship in, and leave the milestone empty until the issue is planned for a release. Epics don't get milestones, because
their sub-issues often ship in different releases.

## Triage

New issues get `status: needs-triage` (the issue forms add it) and land in Backlog. Triage means:

- roughly estimating the size (XS-XL)
- setting Priority and Impact, assigning `topic:` labels
- confirming that a reported bug is reproducible
- deciding if the issue is invalid, a duplicate, superseded, won't be done, or a valid task
- attaching it to an epic and, if it's planned for a release, setting the milestone

Remove `status: needs-triage` once that's done. The project's Triage view lists the items still waiting.

## Board flow

1. **Backlog**: new issues land here uncategorized and unplanned. Size can be set here or later.
2. **Ready to work on**: the issue is clear and we decided to do it. Every issue here has a Size and a Priority.
3. **In progress**: pulled when work starts. Take the highest priority issue, or balance priority against size.
4. **In review**: the change is done and a PR is open.
5. **Post-merge**: merged, but follow-up work remains: docs, CI patches, follow-up issues, label cleanup. Move to Done
   by hand once finished.
6. **Done**: works, is tested and merged into `master`.

A task lasts at most half a sprint (about a week), so tasks are XS or S. Anything M or larger is a story, feature or
epic, or gets split into tasks.
