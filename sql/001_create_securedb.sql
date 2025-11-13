IF DB_ID('SecureDb') IS NULL
    CREATE DATABASE SecureDb;
GO
USE SecureDb;
GO
IF OBJECT_ID('dbo.Users', 'U') IS NOT NULL
    DROP TABLE dbo.Users;
GO
CREATE TABLE dbo.Users (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(88) NOT NULL,
    FullName NVARCHAR(100) NULL,
    Email NVARCHAR(100) NULL,
    PhoneEncrypted NVARCHAR(MAX) NULL,
    AddressEncrypted NVARCHAR(MAX) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    UpdatedAt DATETIME2 NULL
);
GO
INSERT INTO dbo.Users (
    Username,
    PasswordHash,
    FullName,
    Email,
    PhoneEncrypted,
    AddressEncrypted
) VALUES (
    N'admin',
    N'6G94qKPK8LYNjnTllCqm2G3BUM08AzOK7yW30tfjrMc=',
    N'Administrator',
    N'admin@example.com',
    N'PLACEHOLDER_PHONE_ENCRYPTED',
    N'PLACEHOLDER_ADDRESS_ENCRYPTED'
);
