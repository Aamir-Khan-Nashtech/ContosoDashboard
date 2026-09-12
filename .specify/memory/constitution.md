<!--
Sync Impact Report
- Version change: unversioned → 1.0.0
- Modified principles: new constitution baseline (placeholder template replaced with project-specific rules)
- Added sections: Core Principles, Additional Constraints, Development Workflow, Governance
- Removed sections: placeholder [PROJECT_NAME], [PRINCIPLE_*], and [SECTION_*] tokens
- Templates requiring updates: .specify/templates/plan-template.md ⚠ pending; .specify/templates/spec-template.md ⚠ pending; .specify/templates/tasks-template.md ⚠ pending; .specify/templates/commands/*.md ⚠ pending (directory not present in this repo)
- Deferred items: TODO(RATIFICATION_DATE): original adoption date was not recorded for this repository
-->

# ContosoDashboard Constitution

## Core Principles

### I. Security-First Learning Architecture
This project MUST prioritize explicit authorization, user isolation, and secure defaults in every feature. Features that expose project, task, or notification data MUST enforce page-level authorization and service-level validation before returning data. Authentication is training-only and MUST NOT be treated as production-ready; any production migration MUST use real identity providers and stronger secrets.

Rationale: The repository intentionally demonstrates mock authentication and RBAC for training, so the rule is to keep security boundaries explicit and reviewable.

### II. User-Scoped Data Access
No user may access records outside their permitted scope through URLs, query parameters, or service methods. Custom access checks MUST validate membership or role before exposing tasks, projects, profiles, or notifications. If a record is not authorized for the current user, the system MUST deny access and follow the standard unauthenticated or unauthorized flow.

Rationale: This prevents insecure direct object reference patterns and supports the project’s training security examples.

### III. Test-First Feature Delivery
For any functional change, a failing test or concrete verification scenario MUST exist before implementation. Changes to authentication, authorization, task flows, project access, and notification behavior MUST be validated with a live user-flow check or automated test. The implementation sequence is red → green → refactor.

Rationale: Reliable validation keeps training examples demonstrably correct and reduces regressions in permission-sensitive features.

### IV. Offline-First, Cloud-Migration-Friendly Design
The application MUST remain runnable offline with local persistence and local identity simulation. New infrastructure dependencies and storage mechanisms MUST be abstracted behind interfaces so that domain logic remains decoupled from the local/offline implementation details. Any cloud migration MUST be configuration-driven rather than a business-logic rewrite.

Rationale: This keeps the training app usable without Azure or external services while preserving a clear path to production-ready cloud patterns.

### V. Clear Separation of Concerns
The app MUST separate data models, services, UI pages, and infrastructure responsibilities. Business rules MUST live in services and not in Razor pages; UI code MUST NOT bypass authorization or data access checks. Cross-cutting logic such as authentication, notifications, and dashboard aggregation MUST remain explicit and maintainable.

Rationale: Clear boundaries keep the codebase understandable for spec-driven learning and reduce accidental coupling.

## Additional Constraints

- The project is a training artifact and MUST NOT be promoted as production-ready infrastructure.
- The app MUST use local-only development defaults unless explicitly documented otherwise.
- Role-based access MUST reflect user permissions: Administrator, Project Manager, Team Lead, and Employee.
- Any file upload or document feature MUST generate unique storage paths and validate ownership before write or delete operations.
- Security headers, cookie protections, and claims-based access checks MUST remain enabled unless a training exercise intentionally removes them for demonstration.
- Code and documentation MUST clearly label mock identity and intentionally simplified security controls as training-only.

## Development Workflow

- Features MUST be specified before implementation, using user stories, acceptance scenarios, and measurable outcomes.
- Each story MUST declare its independent test or validation path before development begins.
- Pull requests MUST confirm that relevant security checks, role assumptions, and user-scope constraints remain valid.
- Changes to authentication or authorization that add roles or permissions MUST also update seeded demo data and user-facing documentation.
- Changes to data models or services MUST preserve the current demo scenario and MUST NOT introduce dependencies on external services unless explicitly planned.

## Governance
This Constitution supersedes informal conventions and repository guidance for the ContosoDashboard training project. All changes to behavior, data access, or security boundaries MUST be reviewed against these principles before merge.

Amendments require:
1. A documented reason for the change, including the principle affected and the impact on the training scenario.
2. A version bump according to the policy below.
3. A review confirming that no user-scope, authorization, or migration assumptions were weakened.
4. Updates to relevant implementation or documentation artifacts when the change alters workflow, security, or architecture.

Versioning policy:
- MAJOR: backward-incompatible changes to governance, required principles, or non-negotiable security requirements.
- MINOR: new principle or major clarifying guidance that expands required behavior without removing existing guarantees.
- PATCH: wording, examples, typo corrections, or small clarifications that do not change required behavior.

Compliance review:
- Reviewers MUST verify user-isolation logic, authorization checks, and migration boundaries remain intact.
- Feature work MUST include a clear verification step showing success for the affected user story.
- Any deviation from the training-safe architecture or offline-first defaults MUST be documented and justified in writing.

**Version**: 1.0.0 | **Ratified**: TODO(RATIFICATION_DATE): original adoption date was not recorded for this repository | **Last Amended**: 2026-09-12
