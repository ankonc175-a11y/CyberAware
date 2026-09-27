
CREATE DATABASE CybersecurityTraining;
GO

USE CybersecurityTraining;
GO

/* 1. Drop existing tables in reverse dependency order */
DROP TABLE IF EXISTS Answers;
DROP TABLE IF EXISTS Options;
DROP TABLE IF EXISTS Schedules;
DROP TABLE IF EXISTS Module_Versions;
DROP TABLE IF EXISTS Questions;
DROP TABLE IF EXISTS Content_Sections;
DROP TABLE IF EXISTS Attempts;
DROP TABLE IF EXISTS Notifications;
DROP TABLE IF EXISTS Audit_Logs;
DROP TABLE IF EXISTS Enrolments;
DROP TABLE IF EXISTS Modules;
DROP TABLE IF EXISTS Users;
DROP TABLE IF EXISTS Roles;
GO

/* 2. Roles */
CREATE TABLE Roles
(
    RoleID      INT             IDENTITY(1,1) NOT NULL,
    RoleName    NVARCHAR(50)    NOT NULL,

    CONSTRAINT PK_Roles PRIMARY KEY (RoleID),
    CONSTRAINT UQ_Roles_RoleName UNIQUE (RoleName)
);
GO

/* 3. Users */
CREATE TABLE Users
(
    UserID              INT             IDENTITY(1,1) NOT NULL,
    Email               NVARCHAR(255)   NOT NULL,
    PasswordHash        NVARCHAR(255)   NOT NULL,
    IsTwoFactorEnabled  BIT             NOT NULL
        CONSTRAINT DF_Users_IsTwoFactorEnabled DEFAULT (0),
    TwoFactorSecret     NVARCHAR(255)   NULL,
    RoleID              INT             NOT NULL,
    FirstName           NVARCHAR(100)   NOT NULL,
    LastName            NVARCHAR(100)   NOT NULL,
    IsActive            BIT             NOT NULL
        CONSTRAINT DF_Users_IsActive DEFAULT (1),
    CreatedAt           DATETIME2(0)    NOT NULL
        CONSTRAINT DF_Users_CreatedAt DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Users PRIMARY KEY (UserID),
    CONSTRAINT UQ_Users_Email UNIQUE (Email),
    CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleID)
        REFERENCES Roles (RoleID)
        ON UPDATE NO ACTION ON DELETE NO ACTION
);
GO

/* 4. Modules */
CREATE TABLE Modules
(
    ModuleID        INT             IDENTITY(1,1) NOT NULL,
    Title           NVARCHAR(200)   NOT NULL,
    Description     NVARCHAR(MAX)   NULL,
    CreatedBy       INT             NOT NULL,
    IsPublished     BIT             NOT NULL
        CONSTRAINT DF_Modules_IsPublished DEFAULT (0),
    CreatedAt       DATETIME2(0)    NOT NULL
        CONSTRAINT DF_Modules_CreatedAt DEFAULT (SYSDATETIME()),
    UpdatedAt       DATETIME2(0)    NOT NULL
        CONSTRAINT DF_Modules_UpdatedAt DEFAULT (SYSDATETIME()),
	PassMark		INT				NOT NULL,

    CONSTRAINT PK_Modules PRIMARY KEY (ModuleID),
    CONSTRAINT FK_Modules_CreatedBy FOREIGN KEY (CreatedBy)
        REFERENCES Users (UserID)
        ON UPDATE NO ACTION ON DELETE NO ACTION
);
GO

/* 5. Enrolments */
CREATE TABLE Enrolments
(
    EnrolmentID     INT             IDENTITY(1,1) NOT NULL,
    UserID          INT             NOT NULL,
    ModuleID        INT             NOT NULL,
    EnrolledBy      INT             NOT NULL,
    PathwayName     NVARCHAR(150)   NULL,
    EnrolledAt      DATETIME2(0)    NOT NULL
        CONSTRAINT DF_Enrolments_EnrolledAt DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Enrolments PRIMARY KEY (EnrolmentID),
    CONSTRAINT FK_Enrolments_User FOREIGN KEY (UserID)
        REFERENCES Users (UserID)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
    CONSTRAINT FK_Enrolments_Module FOREIGN KEY (ModuleID)
        REFERENCES Modules (ModuleID)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
    CONSTRAINT FK_Enrolments_EnrolledBy FOREIGN KEY (EnrolledBy)
        REFERENCES Users (UserID)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
    CONSTRAINT UQ_Enrolments_User_Module UNIQUE (UserID, ModuleID)
);
GO

/* 6. Audit Logs */
CREATE TABLE Audit_Logs
(
    LogID           INT             IDENTITY(1,1) NOT NULL,
    UserID          INT             NOT NULL,
    Action          NVARCHAR(100)   NOT NULL,
    TargetTable     NVARCHAR(100)   NULL,
    TargetID        INT             NULL,
    Details         NVARCHAR(MAX)   NULL,
    [Timestamp]     DATETIME2(0)    NOT NULL
        CONSTRAINT DF_AuditLogs_Timestamp DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Audit_Logs PRIMARY KEY (LogID),
    CONSTRAINT FK_AuditLogs_User FOREIGN KEY (UserID)
        REFERENCES Users (UserID)
        ON UPDATE NO ACTION ON DELETE NO ACTION
);
GO

/* 7. Notifications */
CREATE TABLE Notifications
(
    NotificationID  INT             IDENTITY(1,1) NOT NULL,
    UserID          INT             NOT NULL,
    Message         NVARCHAR(500)   NOT NULL,
    IsRead          BIT             NOT NULL
        CONSTRAINT DF_Notifications_IsRead DEFAULT (0),
    CreatedAt       DATETIME2(0)    NOT NULL
        CONSTRAINT DF_Notifications_CreatedAt DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Notifications PRIMARY KEY (NotificationID),
    CONSTRAINT FK_Notifications_User FOREIGN KEY (UserID)
        REFERENCES Users (UserID)
        ON UPDATE NO ACTION ON DELETE CASCADE
);
GO

/* 8. Content Sections */
CREATE TABLE Content_Sections
(
    SectionID       INT             IDENTITY(1,1) NOT NULL,
    ModuleID        INT             NOT NULL,
    Title           NVARCHAR(200)   NOT NULL,
    ContentBody     NVARCHAR(MAX)   NOT NULL,
    OrderIndex      INT             NOT NULL,

    CONSTRAINT PK_Content_Sections PRIMARY KEY (SectionID),
    CONSTRAINT FK_ContentSections_Module FOREIGN KEY (ModuleID)
        REFERENCES Modules (ModuleID)
        ON UPDATE NO ACTION ON DELETE CASCADE,
    CONSTRAINT CK_ContentSections_OrderIndex CHECK (OrderIndex >= 0),
    CONSTRAINT UQ_ContentSections_Module_Order UNIQUE (ModuleID, OrderIndex)
);
GO

/* 9. Questions */
CREATE TABLE Questions
(
    QuestionID				INT             IDENTITY(1,1) NOT NULL,
    ModuleID				INT             NOT NULL,
    QuestionText			NVARCHAR(MAX)   NOT NULL,
    DifficultyLevel			NVARCHAR(20)    NOT NULL,
	QuestionExplanation		NVARCHAR(MAX)   NOT NULL,

    CONSTRAINT PK_Questions PRIMARY KEY (QuestionID),
    CONSTRAINT FK_Questions_Module FOREIGN KEY (ModuleID)
        REFERENCES Modules (ModuleID)
        ON UPDATE NO ACTION ON DELETE CASCADE,
    CONSTRAINT CK_Questions_Difficulty CHECK
        (DifficultyLevel IN ('Easy', 'Medium', 'Hard'))
);
GO

/* 10. Options */
CREATE TABLE Options
(
    OptionID        INT             IDENTITY(1,1) NOT NULL,
    QuestionID      INT             NOT NULL,
    OptionText      NVARCHAR(500)   NOT NULL,
    IsCorrect       BIT             NOT NULL
        CONSTRAINT DF_Options_IsCorrect DEFAULT (0),

    CONSTRAINT PK_Options PRIMARY KEY (OptionID),
    CONSTRAINT FK_Options_Question FOREIGN KEY (QuestionID)
        REFERENCES Questions (QuestionID)
        ON UPDATE NO ACTION ON DELETE CASCADE
);
GO

/* 11. Attempts */
CREATE TABLE Attempts
(
    AttemptID        INT             IDENTITY(1,1) NOT NULL,
    UserID           INT             NOT NULL,
    ModuleID         INT             NOT NULL,
    Score            FLOAT           NOT NULL,
    Passed           BIT             NOT NULL,
    AttemptDate      DATETIME2(0)    NOT NULL
        CONSTRAINT DF_Attempts_AttemptDate DEFAULT (SYSDATETIME()),
    DurationSeconds  INT             NOT NULL,

    CONSTRAINT PK_Attempts PRIMARY KEY (AttemptID),
    CONSTRAINT FK_Attempts_User FOREIGN KEY (UserID)
        REFERENCES Users (UserID)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
    CONSTRAINT FK_Attempts_Module FOREIGN KEY (ModuleID)
        REFERENCES Modules (ModuleID)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
    CONSTRAINT CK_Attempts_Score CHECK (Score >= 0 AND Score <= 100),
    CONSTRAINT CK_Attempts_Duration CHECK (DurationSeconds >= 0)
);
GO

/* 12. Answers */
CREATE TABLE Answers
(
    AnswerID          INT             IDENTITY(1,1) NOT NULL,
    AttemptID         INT             NOT NULL,
    QuestionID        INT             NOT NULL,
    SelectedOptionID  INT             NULL,

    CONSTRAINT PK_Answers PRIMARY KEY (AnswerID),
    CONSTRAINT FK_Answers_Attempt FOREIGN KEY (AttemptID)
        REFERENCES Attempts (AttemptID)
        ON UPDATE NO ACTION ON DELETE CASCADE,
    CONSTRAINT FK_Answers_Question FOREIGN KEY (QuestionID)
        REFERENCES Questions (QuestionID)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
    CONSTRAINT FK_Answers_SelectedOption FOREIGN KEY (SelectedOptionID)
        REFERENCES Options (OptionID)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
    CONSTRAINT UQ_Answers_Attempt_Question UNIQUE (AttemptID, QuestionID)
);
GO

/* 13. Schedules */
CREATE TABLE Schedules
(
    ScheduleID      INT             IDENTITY(1,1) NOT NULL,
    ModuleID        INT             NOT NULL,
    EnrolmentID     INT             NOT NULL,
    FrequencyDays   INT             NOT NULL,
    NextDueDate     DATETIME2(0)    NOT NULL,

    CONSTRAINT PK_Schedules PRIMARY KEY (ScheduleID),
    CONSTRAINT FK_Schedules_Module FOREIGN KEY (ModuleID)
        REFERENCES Modules (ModuleID)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
    CONSTRAINT FK_Schedules_Enrolment FOREIGN KEY (EnrolmentID)
        REFERENCES Enrolments (EnrolmentID)
        ON UPDATE NO ACTION ON DELETE CASCADE,
    CONSTRAINT CK_Schedules_FrequencyDays CHECK (FrequencyDays > 0)
);
GO

/* 14. Module Versions */
CREATE TABLE Module_Versions
(
    VersionID        INT             IDENTITY(1,1) NOT NULL,
    ModuleID         INT             NOT NULL,
    VersionNumber    INT             NOT NULL,
    ContentSnapshot  NVARCHAR(MAX)   NOT NULL,
    CreatedBy        INT             NOT NULL,
    CreatedAt        DATETIME2(0)    NOT NULL
        CONSTRAINT DF_ModuleVersions_CreatedAt DEFAULT (SYSDATETIME()),

    CONSTRAINT PK_Module_Versions PRIMARY KEY (VersionID),
    CONSTRAINT FK_ModuleVersions_Module FOREIGN KEY (ModuleID)
        REFERENCES Modules (ModuleID)
        ON UPDATE NO ACTION ON DELETE CASCADE,
    CONSTRAINT FK_ModuleVersions_CreatedBy FOREIGN KEY (CreatedBy)
        REFERENCES Users (UserID)
        ON UPDATE NO ACTION ON DELETE NO ACTION,
    CONSTRAINT CK_ModuleVersions_VersionNumber CHECK (VersionNumber > 0),
    CONSTRAINT UQ_ModuleVersions_Module_Version UNIQUE (ModuleID, VersionNumber)
);
GO

CREATE INDEX IX_Users_RoleID ON Users (RoleID);
CREATE INDEX IX_Modules_CreatedBy ON Modules (CreatedBy);
CREATE INDEX IX_Enrolments_UserID ON Enrolments (UserID);
CREATE INDEX IX_Enrolments_ModuleID ON Enrolments (ModuleID);
CREATE INDEX IX_Attempts_UserID ON Attempts (UserID);
CREATE INDEX IX_Attempts_ModuleID ON Attempts (ModuleID);
CREATE INDEX IX_Questions_ModuleID ON Questions (ModuleID);
CREATE INDEX IX_Options_QuestionID ON Options (QuestionID);
CREATE INDEX IX_Notifications_UserID ON Notifications (UserID);
GO
