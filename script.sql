CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" TEXT NOT NULL CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY,
    "ProductVersion" TEXT NOT NULL
);

BEGIN TRANSACTION;

CREATE TABLE "UserProfiles" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_UserProfiles" PRIMARY KEY,
    "Name" TEXT NOT NULL,
    "Email" TEXT NOT NULL,
    "WorkMode" INTEGER NOT NULL,
    "DailyFocusGoalMinutes" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL
);

CREATE TABLE "FocusSessions" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_FocusSessions" PRIMARY KEY,
    "UserProfileId" TEXT NOT NULL,
    "Title" TEXT NOT NULL,
    "Description" TEXT NULL,
    "FocusMode" INTEGER NOT NULL,
    "PlannedStart" TEXT NOT NULL,
    "StartedAt" TEXT NULL,
    "CompletedAt" TEXT NULL,
    "Status" INTEGER NOT NULL,
    "CancellationReason" TEXT NULL,
    "ActualDurationMinutes" INTEGER NULL,
    "PlannedDurationMinutes" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    CONSTRAINT "FK_FocusSessions_UserProfiles_UserProfileId" FOREIGN KEY ("UserProfileId") REFERENCES "UserProfiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE "BreakPeriods" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_BreakPeriods" PRIMARY KEY,
    "FocusSessionId" TEXT NOT NULL,
    "BreakType" INTEGER NOT NULL,
    "StartedAt" TEXT NOT NULL,
    "Notes" TEXT NULL,
    "DurationMinutes" INTEGER NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    CONSTRAINT "FK_BreakPeriods_FocusSessions_FocusSessionId" FOREIGN KEY ("FocusSessionId") REFERENCES "FocusSessions" ("Id") ON DELETE CASCADE
);

CREATE TABLE "DeviceSignals" (
    "Id" TEXT NOT NULL CONSTRAINT "PK_DeviceSignals" PRIMARY KEY,
    "FocusSessionId" TEXT NOT NULL,
    "DeviceName" TEXT NOT NULL,
    "SignalType" TEXT NOT NULL,
    "Payload" TEXT NOT NULL,
    "RecordedAt" TEXT NOT NULL,
    "CreatedAt" TEXT NOT NULL,
    "UpdatedAt" TEXT NULL,
    CONSTRAINT "FK_DeviceSignals_FocusSessions_FocusSessionId" FOREIGN KEY ("FocusSessionId") REFERENCES "FocusSessions" ("Id") ON DELETE CASCADE
);

CREATE INDEX "IX_BreakPeriods_FocusSessionId" ON "BreakPeriods" ("FocusSessionId");

CREATE INDEX "IX_DeviceSignals_FocusSessionId" ON "DeviceSignals" ("FocusSessionId");

CREATE INDEX "IX_FocusSessions_UserProfileId" ON "FocusSessions" ("UserProfileId");

CREATE UNIQUE INDEX "IX_UserProfiles_Email" ON "UserProfiles" ("Email");

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20251112161533_InitialCreate', '8.0.10');

COMMIT;

