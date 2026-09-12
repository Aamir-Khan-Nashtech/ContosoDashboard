# Data Model: Document Upload and Management

## Overview

The document feature will add a small set of entities to the existing ContosoDashboard data model. The design keeps document metadata in the EF Core database and stores file content separately on disk.

## Entities

### Document

Represents a stored document and its metadata.

| Field | Type | Constraints | Notes |
|---|---|---|---|
| DocumentId | int | PK, required | Matches existing integer identity pattern |
| Title | string | required, max 255 | User-facing title |
| Description | string? | optional, max 2000 | Optional summary |
| Category | string | required, max 100 | Example values: Project Documents, Team Resources, Personal Files, Reports, Presentations, Other |
| FileName | string | required, max 255 | Original file name, not used directly in storage path |
| StoredFileName | string | required, max 255 | GUID-based generated file name |
| FilePath | string | required, max 500 | Relative path within local storage root |
| FileType | string | required, max 255 | MIME type such as application/pdf |
| FileSizeBytes | long | required | Original file size |
| UploadedByUserId | int | required, FK to User | Uploader |
| ProjectId | int? | nullable, FK to Project | Optional project association |
| TaskId | int? | nullable, FK to TaskItem | Optional task association |
| UploadDateUtc | DateTime | required | Upload timestamp |
| UpdatedDateUtc | DateTime | required | Last metadata update |
| IsDeleted | bool | required default false | Soft-delete flag if needed for audit retention |

**Relationships**:
- Many documents belong to one uploader (`User`)
- Many documents may belong to zero or one project (`Project`)
- Many documents may belong to zero or one task (`TaskItem`)
- One document may have many share records
- One document may have many activity events

### DocumentShare

Tracks explicit sharing permissions for a document.

| Field | Type | Constraints | Notes |
|---|---|---|---|
| DocumentShareId | int | PK | Unique share record |
| DocumentId | int | FK to Document | Shared document |
| SharedWithUserId | int? | FK to User | Direct user share |
| SharedWithProjectId | int? | FK to Project | Project/team-group share |
| SharedByUserId | int | FK to User | Owner or manager who granted access |
| SharedDateUtc | DateTime | required | When share was granted |
| IsActive | bool | required default true | Allows revoke without deletion |

**Relationships**:
- One document has many share entries
- One user may share many documents
- One project may receive many shared document grants

### DocumentActivity

Captures audit and reporting events.

| Field | Type | Constraints | Notes |
|---|---|---|---|
| DocumentActivityId | int | PK | Unique event |
| DocumentId | int | FK to Document | Related document |
| UserId | int | FK to User | User who performed the action |
| ActionType | string | required, max 50 | Upload, download, view, delete, share, update |
| ActionDateUtc | DateTime | required | Event timestamp |
| Details | string? | optional, max 1000 | Additional context |

**Relationships**:
- One document has many activity entries
- One user performs many activity records

## Validation Rules

- `Title` is required and should be trimmed before persistence.
- `Category` must be one of the configured values: Project Documents, Team Resources, Personal Files, Reports, Presentations, Other.
- `FileType` must be 255 characters or fewer to accommodate Microsoft Office MIME values.
- `FilePath` must be generated before database persistence and must never use user-provided file names directly.
- `ProjectId` may be null for personal documents.
- `TaskId` may be null unless the document is attached directly to a task.
- `UploadedByUserId` must match the authenticated user performing the upload.
- `SharedWithUserId` and `SharedWithProjectId` cannot both be null for a share record.

## State Transitions

### Document lifecycle

- Draft/Queued → Upload validation in service
- Stored → metadata saved and file content persisted
- Active → visible to authorized users
- Updated → metadata or file replaced
- Deleted → removed from access list and activity log in final state

### Share lifecycle

- Created → active grant added to `DocumentShare`
- Revoked → `IsActive` set false
- Expired or removed → no longer included in active access checks

## Access Rules

The access model should enforce the following before returning a document or its file:

- Document owner may always view and update own documents
- Project manager may manage project documents for the associated project
- Team lead or project team member may view project documents
- Shared users may access documents granted explicitly
- Administrators can access all documents for audit uses

This logic should live in the service layer and should be mirrored by page-level authorization checks in the UI.
