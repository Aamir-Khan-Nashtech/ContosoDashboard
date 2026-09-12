# Quickstart: Document Upload and Management

## Prerequisites

- .NET 8 SDK installed
- Local SQL Server LocalDB available to the app
- Visual Studio 2022 or VS Code
- Existing ContosoDashboard application running locally

## Run the app

1. Open the solution in the ContosoDashboard project folder.
2. Restore packages and build the app.
3. Start the app with the normal development command:

```bash
dotnet run
```

4. Navigate to the login page and sign in as an existing demo user.
5. Open the dashboard and validate that the new document experience is available in the relevant navigation area.

## Feature validation flow

### Upload a document

1. Open the document management page.
2. Select a supported file such as a PDF or Word document.
3. Enter a title, category, description, and optional project association.
4. Submit the upload.
5. Confirm the document appears in the user’s document list.

### Validate permissions

1. Upload a project document as a project manager.
2. Sign in as a different project member.
3. Ensure the document is visible within the project view.
4. Sign in as a user without project access.
5. Confirm the document is not shown and access is denied.

### Share a document

1. Open an uploaded document.
2. Share it with a specific user or a project/team group.
3. Sign in as the recipient.
4. Confirm the document appears in the shared-with-me section and a notification is generated.

### Update and delete

1. Edit metadata for a document the user owns.
2. Replace the underlying file with a new version.
3. Delete the document after confirmation.
4. Confirm the file is removed from storage and the metadata record is no longer visible.

## Expected behavior summary

- Upload must validate file extension and size before persistence.
- Files must land under a local storage root outside the web root.
- Shared documents must appear only to authorized users.
- Download, preview, and delete endpoints must enforce both service-level and page-level authorization.
- Document activity logs should capture each major action for audit purposes.
