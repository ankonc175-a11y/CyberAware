
USE CybersecurityTraining;
GO

/* Ensure the role names used exist. Existing roles are kept. */
IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = N'User')
    INSERT INTO Roles (RoleName) VALUES (N'User');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = N'Module Owner')
    INSERT INTO Roles (RoleName) VALUES (N'Module Owner');
IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = N'Admin')
    INSERT INTO Roles (RoleName) VALUES (N'Admin');
GO

/* Register */
CREATE OR ALTER PROCEDURE RegisterUser
    @Email        NVARCHAR(255), 
    @PasswordHash NVARCHAR(255),
    @FirstName    NVARCHAR(100),
    @LastName     NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SET @Email = LTRIM(RTRIM(@Email));
    SET @FirstName = LTRIM(RTRIM(@FirstName));
    SET @LastName = LTRIM(RTRIM(@LastName));

    IF NULLIF(@Email, N'') IS NULL OR NULLIF(@PasswordHash, N'') IS NULL
       OR NULLIF(@FirstName, N'') IS NULL OR NULLIF(@LastName, N'') IS NULL
        THROW 50001, 'Email, password hash, first name and last name are required.', 1;

    DECLARE @RoleID INT;
    SELECT @RoleID = RoleID FROM Roles WHERE RoleName = N'User';
    IF @RoleID IS NULL
        THROW 50002, 'The User role does not exist.', 1;

    IF EXISTS (SELECT 1 FROM Users WHERE Email = @Email)
        THROW 50003, 'Email already registered.', 1;

    /* The existing Users table supplies UserID, IsActive, 2FA flag and CreatedAt defaults. */
    INSERT INTO Users (Email, PasswordHash, RoleID, FirstName, LastName)
    VALUES (@Email, @PasswordHash, @RoleID, @FirstName, @LastName);

    SELECT CAST(SCOPE_IDENTITY() AS INT) AS NewUserID;
END;
GO

/* Get User info by mail */
CREATE OR ALTER PROCEDURE GetUserByEmail
    @Email NVARCHAR(255)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, Email, PasswordHash, RoleID, FirstName, LastName,
           IsActive, IsTwoFactorEnabled
    FROM Users
    WHERE Email = LTRIM(RTRIM(@Email));
END;
GO

/* Public-facing profile fields */
CREATE OR ALTER PROCEDURE GetUserProfile
    @UserID INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT u.UserID, u.Email, u.FirstName, u.LastName,
           u.RoleID, r.RoleName, u.IsActive, u.IsTwoFactorEnabled, u.CreatedAt
    FROM Users AS u
    INNER JOIN Roles AS r ON r.RoleID = u.RoleID
    WHERE u.UserID = @UserID;
END;
GO

/* Edit only profile fields. Email remains unchanged */
CREATE OR ALTER PROCEDURE UpdateUserProfile
    @UserID INT,
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SET @FirstName = LTRIM(RTRIM(@FirstName));
    SET @LastName = LTRIM(RTRIM(@LastName));

    IF NULLIF(@FirstName, N'') IS NULL OR NULLIF(@LastName, N'') IS NULL
        THROW 50004, 'First name and last name are required.', 1;

    UPDATE Users
    SET FirstName = @FirstName, LastName = @LastName
    WHERE UserID = @UserID;

    IF @@ROWCOUNT = 0
        THROW 50005, 'User not found.', 1;
END;
GO

/* Admin-only: Checking users info */
CREATE OR ALTER PROCEDURE GetAllUsers
    @AdminUserID INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (
        SELECT 1 FROM Users AS u
        INNER JOIN Roles AS r ON r.RoleID = u.RoleID
        WHERE u.UserID = @AdminUserID AND u.IsActive = 1 AND r.RoleName = N'Admin'
    ) THROW 50006, 'Active administrator required.', 1; /* Check if the person request this ifo is admin */
	
    SELECT u.UserID, u.Email, u.FirstName, u.LastName,
           u.RoleID, r.RoleName, u.IsActive, u.CreatedAt
    FROM Users AS u
    INNER JOIN Roles AS r ON r.RoleID = u.RoleID
    ORDER BY u.UserID;
END;
GO

/* Admin-only: change a user's role. */
CREATE OR ALTER PROCEDURE UpdateUserRole
    @AdminUserID INT,
    @UserID INT,
    @NewRoleID INT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (
        SELECT 1 FROM Users AS u INNER JOIN Roles AS r ON r.RoleID = u.RoleID
        WHERE u.UserID = @AdminUserID AND u.IsActive = 1 AND r.RoleName = N'Admin'
    ) THROW 50007, 'Active administrator required.', 1;

    IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleID = @NewRoleID)
        THROW 50008, 'New role does not exist.', 1;

    IF @AdminUserID = @UserID
        THROW 50009, 'An administrator cannot change their own role here.', 1;

    UPDATE Users SET RoleID = @NewRoleID WHERE UserID = @UserID;
    IF @@ROWCOUNT = 0
        THROW 50010, 'User not found.', 1;
END;
GO

/* Admin-only: activate or deactivate an account. */
CREATE OR ALTER PROCEDURE SetUserActiveStatus
    @AdminUserID INT,
    @UserID INT,
    @IsActive BIT
AS
BEGIN
    SET NOCOUNT ON;
    IF NOT EXISTS (
        SELECT 1 FROM Users AS u INNER JOIN Roles AS r ON r.RoleID = u.RoleID
        WHERE u.UserID = @AdminUserID AND u.IsActive = 1 AND r.RoleName = N'Admin'
    ) THROW 50011, 'Active administrator required.', 1;

    IF @AdminUserID = @UserID AND @IsActive = 0
        THROW 50012, 'An administrator cannot deactivate their own account here.', 1;

    UPDATE Users SET IsActive = @IsActive WHERE UserID = @UserID;
    IF @@ROWCOUNT = 0
        THROW 50013, 'User not found.', 1;
END;
GO


