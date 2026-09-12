# Research: Document Upload and Management

## Summary

The feature needs to fit the existing ContosoDashboard training app without introducing a major platform change. The design resolves around three patterns already supported by the repository: EF Core metadata storage, service-layer authorization, and file storage outside the web root with abstraction for future Azure migration.

## Decisions

### Decision: Use local filesystem storage with an interface abstraction

The file content will be stored in a dedicated local directory under the application root (for example, AppData/uploads). The domain logic will call a `IFileStorageService` interface instead of directly interacting with the filesystem. The initial implementation will use `LocalFileStorageService` in the training environment, while future Azure migration can swap in `AzureBlobStorageService` without changing the UI or business services.

**Rationale**: This matches the project requirement for offline-first behavior and the architecture notes already documented in the README. It also aligns with the training objective of showing infrastructure abstraction.

**Alternatives considered**:
- Storing uploaded files inside `wwwroot`: rejected because it exposes file content directly and violates the project’s security guidance.
- Using a database blob column: rejected because it is less portable and does not align with the intended local-file migration path.

### Decision: Keep metadata in EF Core and use integer IDs for consistency

Document metadata will be stored in the existing SQL Server LocalDB database context using integer primary keys consistent with the current `UserId` and `ProjectId` patterns. File category values will remain string-based rather than enum-backed to match the training app simplicity and stakeholder requirement.

**Rationale**: This keeps the document system consistent with the repository’s existing schema and avoids large-scale refactoring.

**Alternatives considered**:
- GUID primary keys: rejected because it breaks consistency with the rest of the app and the requirement explicitly calls for integer `DocumentId` values.
- Enum-backed categories: rejected because the feature requirement says categories must be stored as text values for simplicity.

### Decision: Use role-based access plus explicit document shares

Access will be enforced by service methods and page-level authorization. Users may access documents when they are owners, project members, team members in a permitted project, project managers for a project, or recipients of an explicit document share. The share model supports both direct user grants and project/team-group grants.

**Rationale**: This matches the role model already present in the app and the clarified requirement for both individual and project/team sharing.

**Alternatives considered**:
- Open project visibility only: rejected because it would grant more access than allowed and does not support targeted sharing.
- Administrator-only access: rejected because it blocks normal employees and project managers from doing their work.

### Decision: Treat malware scanning as a required validation stage, implemented as a placeholder for training

Document upload must include malware scanning before persistence, but the offline training environment does not require external antivirus infrastructure. The design should include a validation hook in the service, even if the initial implementation uses a placeholder or simulated pass-through check.

**Rationale**: This preserves the requirement without introducing heavy external tooling into the training sample.

**Alternatives considered**:
- Omitting malware scanning entirely: rejected because it violates the explicit requirement and weakens the security pattern.
- Integrating with cloud scanning: rejected because the project must remain offline and local-only for training.

## Key Open Issues Resolved

- Document sharing scope: resolved as both direct user sharing and project/team-group sharing.
- File storage location: resolved as AppData/uploads outside the web root.
- Database pattern: resolved as integer document IDs with text categories.
- Validation flow: resolved as metadata validation, extension checks, size checks, and authorization before persistence.

## Best Practices Applied

- Secure-by-default file paths using GUID-based filenames and sanitation before persistence.
- Validate file existence and ownership before download or delete operations.
- Keep file path generation and storage logic separate from metadata logic.
- Preserve service-level authorization logic instead of trusting the UI alone.
- Record document activity events for auditor and user-facing reporting.
