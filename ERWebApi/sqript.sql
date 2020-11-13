IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

CREATE TABLE [AclVerbs] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [DefaultValue] int NOT NULL,
    [Description] nvarchar(50) NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_AclVerbs] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Customers] (
    [Id] uniqueidentifier NOT NULL,
    [FirstName] nvarchar(50) NULL,
    [LastName] nvarchar(50) NOT NULL,
    [CompanyName] nvarchar(50) NULL,
    [NIP] nvarchar(50) NULL,
    [Email] nvarchar(50) NULL,
    [Email2] nvarchar(50) NULL,
    [PhoneNumber] nvarchar(20) NOT NULL,
    [PhoneNumber2] nvarchar(20) NULL,
    [Description] nvarchar(500) NULL,
    [RowVersion] bigint NOT NULL,
    [DateAdded] DateTime NOT NULL,
    [DateModified] DateTime NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [HardwareTypes] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_HardwareTypes] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Numeration] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(50) NULL,
    [Pattern] nvarchar(max) NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_Numeration] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [OrderStatuses] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [Group] int NOT NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_OrderStatuses] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [OrderTypes] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(max) NOT NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_OrderTypes] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [PrintTemplates] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [Template] nvarchar(max) NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_PrintTemplates] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Roles] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(50) NOT NULL,
    [IsSystem] bit NOT NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [Settings] (
    [Id] uniqueidentifier NOT NULL,
    [Key] nvarchar(max) NULL,
    [Value] nvarchar(max) NULL,
    [ValueType] nvarchar(max) NULL,
    [Category] nvarchar(max) NULL,
    [Description] nvarchar(max) NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_Settings] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [CustomerAddresses] (
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [Street] nvarchar(max) NULL,
    [HouseNumber] nvarchar(max) NULL,
    [City] nvarchar(max) NULL,
    [Postcode] nvarchar(max) NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_CustomerAddresses] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CustomerAddresses_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [CustomItems] (
    [Id] uniqueidentifier NOT NULL,
    [Key] nvarchar(50) NOT NULL,
    [HardwareTypeId] uniqueidentifier NOT NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_CustomItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_CustomItems_HardwareTypes_HardwareTypeId] FOREIGN KEY ([HardwareTypeId]) REFERENCES [HardwareTypes] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [ACLs] (
    [Id] uniqueidentifier NOT NULL,
    [AclVerbId] uniqueidentifier NOT NULL,
    [RoleId] uniqueidentifier NOT NULL,
    [Value] int NOT NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_ACLs] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ACLs_AclVerbs_AclVerbId] FOREIGN KEY ([AclVerbId]) REFERENCES [AclVerbs] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_ACLs_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [Users] (
    [Id] uniqueidentifier NOT NULL,
    [Login] nvarchar(50) NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [Salt] nvarchar(max) NULL,
    [FirstName] nvarchar(50) NULL,
    [LastName] nvarchar(50) NULL,
    [PhoneNumber] nvarchar(50) NULL,
    [IsActive] bit NOT NULL,
    [IsAdmin] bit NOT NULL,
    [IsSystem] bit NOT NULL,
    [RoleId] uniqueidentifier NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Orders] (
    [Id] uniqueidentifier NOT NULL,
    [OrderId] int NOT NULL IDENTITY,
    [CustomerId] uniqueidentifier NULL,
    [Number] nvarchar(50) NULL,
    [DateRegistered] DateTime NOT NULL,
    [DateEnded] DateTime NULL,
    [OrderStatusId] uniqueidentifier NULL,
    [OrderTypeId] uniqueidentifier NULL,
    [UserId] uniqueidentifier NULL,
    [Cost] nvarchar(50) NULL,
    [Fault] nvarchar(1000) NULL,
    [Solution] nvarchar(1000) NULL,
    [Comment] nvarchar(1000) NULL,
    [ExternalNumber] nvarchar(50) NULL,
    [Progress] int NOT NULL,
    [RowVersion] bigint NOT NULL,
    [DateAdded] DateTime NOT NULL,
    [DateModified] DateTime NULL,
    CONSTRAINT [PK_Orders] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Orders_Customers_CustomerId] FOREIGN KEY ([CustomerId]) REFERENCES [Customers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_OrderStatuses_OrderStatusId] FOREIGN KEY ([OrderStatusId]) REFERENCES [OrderStatuses] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_OrderTypes_OrderTypeId] FOREIGN KEY ([OrderTypeId]) REFERENCES [OrderTypes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Orders_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Blob] (
    [Id] uniqueidentifier NOT NULL,
    [FileName] nvarchar(300) NOT NULL,
    [Description] nvarchar(max) NULL,
    [Checksum] nvarchar(max) NULL,
    [Size] int NOT NULL,
    [Data] varbinary(max) NOT NULL,
    [OrderId] uniqueidentifier NULL,
    CONSTRAINT [PK_Blob] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Blob_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [Hardwares] (
    [Id] uniqueidentifier NOT NULL,
    [Name] nvarchar(80) NOT NULL,
    [SerialNumber] nvarchar(80) NOT NULL,
    [HardwareTypeID] uniqueidentifier NULL,
    [RowVersion] bigint NOT NULL,
    [DateAdded] DateTime NOT NULL,
    [DateModified] DateTime NULL,
    [OrderId] uniqueidentifier NULL,
    CONSTRAINT [PK_Hardwares] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Hardwares_HardwareTypes_HardwareTypeID] FOREIGN KEY ([HardwareTypeID]) REFERENCES [HardwareTypes] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Hardwares_Orders_OrderId] FOREIGN KEY ([OrderId]) REFERENCES [Orders] ([Id]) ON DELETE NO ACTION
);

GO

CREATE TABLE [HardwareCustomItems] (
    [Id] uniqueidentifier NOT NULL,
    [CustomItemId] uniqueidentifier NOT NULL,
    [Value] nvarchar(200) NULL,
    [HardwareId] uniqueidentifier NOT NULL,
    [RowVersion] bigint NOT NULL,
    CONSTRAINT [PK_HardwareCustomItems] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HardwareCustomItems_CustomItems_CustomItemId] FOREIGN KEY ([CustomItemId]) REFERENCES [CustomItems] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_HardwareCustomItems_Hardwares_HardwareId] FOREIGN KEY ([HardwareId]) REFERENCES [Hardwares] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_ACLs_AclVerbId] ON [ACLs] ([AclVerbId]);

GO

CREATE INDEX [IX_ACLs_RoleId] ON [ACLs] ([RoleId]);

GO

CREATE INDEX [IX_Blob_OrderId] ON [Blob] ([OrderId]);

GO

CREATE INDEX [IX_CustomerAddresses_CustomerId] ON [CustomerAddresses] ([CustomerId]);

GO

CREATE INDEX [IX_CustomItems_HardwareTypeId] ON [CustomItems] ([HardwareTypeId]);

GO

CREATE INDEX [IX_HardwareCustomItems_CustomItemId] ON [HardwareCustomItems] ([CustomItemId]);

GO

CREATE INDEX [IX_HardwareCustomItems_HardwareId] ON [HardwareCustomItems] ([HardwareId]);

GO

CREATE INDEX [IX_Hardwares_HardwareTypeID] ON [Hardwares] ([HardwareTypeID]);

GO

CREATE INDEX [IX_Hardwares_OrderId] ON [Hardwares] ([OrderId]);

GO

CREATE INDEX [IX_Orders_CustomerId] ON [Orders] ([CustomerId]);

GO

CREATE INDEX [IX_Orders_OrderStatusId] ON [Orders] ([OrderStatusId]);

GO

CREATE INDEX [IX_Orders_OrderTypeId] ON [Orders] ([OrderTypeId]);

GO

CREATE INDEX [IX_Orders_UserId] ON [Orders] ([UserId]);

GO

CREATE UNIQUE INDEX [IX_Users_Login] ON [Users] ([Login]);

GO

CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20201019164149_Initial', N'3.1.9');

GO

