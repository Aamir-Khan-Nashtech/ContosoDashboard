---

description: "Task list for the document upload and management feature"
---

# Tasks: Document Upload and Management

**Input**: Design documents from `/specs/001-document-upload-management/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Tests are optional for this feature and were not explicitly requested in the specification.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description with file path`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **Single project**: `ContosoDashboard/`, `specs/` at repository root
- Paths below follow the existing Blazor Server project structure already in the repo

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and local storage foundation

- [X] T001 Create the local document storage root in ContosoDashboard/AppData/uploads/
- [X] T002 [P] Add the document storage configuration and upload constraints to ContosoDashboard/Program.cs
- [X] T003 [P] Create the document page and service folders used by the feature under ContosoDashboard/Pages/ and ContosoDashboard/Services/

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core document data model and security infrastructure that MUST be complete before any user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

- [X] T004 Add Document, DocumentShare, and DocumentActivity DbSet registrations and indexing in ContosoDashboard/Data/ApplicationDbContext.cs
- [X] T005 [P] Create the base Document model in ContosoDashboard/Models/Document.cs
- [X] T006 [P] Create the DocumentShare model in ContosoDashboard/Models/DocumentShare.cs
- [X] T007 [P] Create the DocumentActivity model in ContosoDashboard/Models/DocumentActivity.cs
- [X] T008 Create the file-storage abstraction and local implementation in ContosoDashboard/Services/IFileStorageService.cs and ContosoDashboard/Services/LocalFileStorageService.cs
- [X] T009 Create the document service contract and core validation flow in ContosoDashboard/Services/DocumentService.cs
- [X] T010 Add database migration-friendly model configuration and relationship rules in ContosoDashboard/Data/ApplicationDbContext.cs

**Checkpoint**: Foundation ready - user story implementation can begin in parallel

---

## Phase 3: User Story 1 - Upload and organize work documents (Priority: P1) 🎯 MVP

**Goal**: Let users upload supported files, assign metadata, and see them in personal or project document lists

**Independent Test**: A logged-in user can upload a valid file, receive confirmation, and view it in the correct document list without leaving the dashboard.

### Implementation for User Story 1

- [X] T011 [P] [US1] Create the document upload page and metadata form in ContosoDashboard/Pages/Documents.razor
- [X] T012 [US1] Implement upload validation, file-extension checks, size enforcement, and unique path generation in ContosoDashboard/Services/DocumentService.cs
- [X] T013 [US1] Implement document persistence to the local file store and EF Core metadata records in ContosoDashboard/Services/DocumentService.cs
- [X] T014 [US1] Add project-linked document listing and category filtering to ContosoDashboard/Pages/ProjectDetails.razor
- [X] T015 [US1] Add the recent-documents widget and summary update to ContosoDashboard/Pages/Index.razor and ContosoDashboard/Services/DashboardService.cs
- [X] T016 [US1] Add upload success/error messaging and user feedback in ContosoDashboard/Pages/Documents.razor
- [X] T017 [US1] Add document metadata and category validation rules to ContosoDashboard/Models/Document.cs and ContosoDashboard/Services/DocumentService.cs

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - Access and manage shared project documents (Priority: P2)

**Goal**: Allow authorized users to search, preview, download, and share documents while enforcing project and ownership rules

**Independent Test**: A user can search for a document, open only authorized results, preview or download permitted files, and share them with another user or project group.

### Implementation for User Story 2

- [ ] T018 [P] [US2] Create the document search/list view and shared-with-me section in ContosoDashboard/Pages/Documents.razor
- [ ] T019 [US2] Implement document retrieval, permission checks, and filtering in ContosoDashboard/Services/DocumentService.cs
- [ ] T020 [US2] Add preview/download authorization and secure file retrieval in ContosoDashboard/Services/DocumentService.cs
- [ ] T021 [US2] Implement share creation and revoke logic in ContosoDashboard/Services/DocumentService.cs
- [ ] T022 [P] [US2] Add explicit share models and user/project share associations in ContosoDashboard/Models/DocumentShare.cs
- [ ] T023 [US2] Integrate in-app notifications for shared documents in ContosoDashboard/Services/NotificationService.cs
- [ ] T024 [US2] Add shared-document visibility in the recipient workflow in ContosoDashboard/Pages/Documents.razor
- [ ] T025 [US2] Enforce project membership and manager authorization before displaying or modifying document data in ContosoDashboard/Services/DocumentService.cs

**Checkpoint**: At this point, User Stories 1 and 2 should both work independently

---

## Phase 5: User Story 3 - Review document activity and maintain audit readiness (Priority: P3)

**Goal**: Provide administrators with auditable document activity, reporting, and governance visibility

**Independent Test**: An administrator can view document activity, identify upload trends, and confirm records are stored for audit use.

### Implementation for User Story 3

- [ ] T026 [P] [US3] Create the document activity model and audit logging flow in ContosoDashboard/Models/DocumentActivity.cs and ContosoDashboard/Services/DocumentService.cs
- [ ] T027 [US3] Record file-related actions (upload, download, update, delete, share) in ContosoDashboard/Services/DocumentService.cs
- [ ] T028 [US3] Add the admin reporting view and document activity summary page in ContosoDashboard/Pages/Reports.razor
- [ ] T029 [US3] Implement aggregate queries for upload counts, top uploaders, and file-type usage in ContosoDashboard/Services/DocumentService.cs
- [ ] T030 [US3] Add admin authorization checks and permissions for reporting access in ContosoDashboard/Services/DocumentService.cs and ContosoDashboard/Program.cs
- [ ] T031 [US3] Add report and activity summary UI wiring in ContosoDashboard/Pages/Index.razor or ContosoDashboard/Pages/Reports.razor

**Checkpoint**: All user stories should now be independently functional

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] T032 [P] Update navigation and route entry points for document management in ContosoDashboard/Shared/NavMenu.razor
- [ ] T033 [P] Add responsive document-table and upload-form styling in ContosoDashboard/wwwroot/css/site.css
- [ ] T034 [P] Validate all document workflows against the requirements in specs/001-document-upload-management/quickstart.md
- [ ] T035 Audit all file paths, access controls, and delete flows against the security guidance in ContosoDashboard/README.md
- [ ] T036 Review the document feature for edge cases such as invalid files, duplicate uploads, unauthorized access, and failed storage operations
- [ ] T037 Final documentation update for document feature behavior in ContosoDashboard/README.md and specs/001-document-upload-management/

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3, 4, 5)**: All depend on Foundational completion
  - User stories can then proceed in parallel if multiple developers are available
  - Recommended order for a single developer: US1 → US2 → US3
- **Polish (Phase 6)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational - independent MVP
- **User Story 2 (P2)**: Can start after Foundational - may integrate with US1 but should remain independently testable
- **User Story 3 (P3)**: Can start after Foundational - may integrate with US1/US2 but should remain independently testable

### Within Each User Story

- Models before services
- Services before pages
- Core implementation before user feedback and reporting
- Story complete before moving to next priority

### Parallel Opportunities

- Setup tasks marked [P] can run in parallel
- Foundational tasks marked [P] can run in parallel
- The upload component and the storage abstraction can be fleshed out in parallel once the model is defined
- Multiple stories can be worked on in parallel by different team members
- Admin reporting and document UI work can progress in parallel after user story 2 core access checks are in place

---

## Parallel Example: User Story 1

```bash
# Launch parallel model work for User Story 1
Task: "Create the base Document model in ContosoDashboard/Models/Document.cs"
Task: "Create the document page and metadata form in ContosoDashboard/Pages/Documents.razor"

# Launch parallel upload work once the foundational model is ready
Task: "Implement upload validation, file-extension checks, size enforcement, and unique path generation in ContosoDashboard/Services/DocumentService.cs"
Task: "Add the recent-documents widget and summary update to ContosoDashboard/Pages/Index.razor and ContosoDashboard/Services/DashboardService.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Confirm upload, metadata persistence, and project list visibility work end-to-end
5. Add User Story 2 next for search and sharing
6. Add User Story 3 last for reporting and audit

### Incremental Delivery

- Deliver upload + organize first as the minimal viable document feature
- Add permission-aware access and share flow next
- Complete admin reporting and audit logging last
- Keep all additions compatible with the current mock-auth, SQL Server LocalDB, and offline-first training architecture
