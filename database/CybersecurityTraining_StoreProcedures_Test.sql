
--Test Procedures

EXEC RegisterUser
    @Email = N'EddyTest@example.com',
    @PasswordHash = N'123Eddy@@',
    @FirstName = N'Eddy', @LastName = N'Tran';

EXEC GetUserByEmail @Email = N'EddyTest@example.com';
EXEC GetUserProfile @UserID = 1;
EXEC UpdateUserProfile @UserID = 1, @FirstName = N'Eddyyyyy', @LastName = N'Trannnn';

--Admin required: AdminUserID = 1, if not this admin, system show error
--Admin: get all info
EXEC GetAllUsers @AdminUserID = 1;
--Change Role
EXEC UpdateUserRole @AdminUserID = 1, @UserID = 2, @NewRoleID = 2;
EXEC GetUserProfile @UserID = 2;
--Change User account activation
EXEC SetUserActiveStatus @AdminUserID = 1, @UserID = 1, @IsActive = 0;
