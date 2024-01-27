CREATE TABLE [dbo].[EmailConfirmations]
(
  [Id] UNIQUEIDENTIFIER DEFAULT(newid()) NOT NULL PRIMARY KEY,
  [Code] UNIQUEIDENTIFIER NOT NULL,
  [UserId] UNIQUEIDENTIFIER NOT NULL FOREIGN KEY REFERENCES [Users](Id) ON DELETE CASCADE,
  [Expires] DATETIMEOFFSET NOT NULL,
  [Active] BIT NOT NULL DEFAULT(1),
  [Created] DATETIMEOFFSET NOT NULL,
  [Updated] DATETIMEOFFSET NULL,
)
