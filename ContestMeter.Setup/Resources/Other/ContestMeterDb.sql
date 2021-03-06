USE [ContestMeterDb]
GO
/****** Object:  Table [dbo].[Administrators]    Script Date: 26.05.2014 19:06:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Administrators](
	[Id] [uniqueidentifier] NOT NULL,
	[AdministratorName] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[Salt] [nvarchar](128) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[LastVisitDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Administrators] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER AUTHORIZATION ON [dbo].[Administrators] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[Contests]    Script Date: 26.05.2014 19:06:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Contests](
	[Id] [uniqueidentifier] NOT NULL,
	[TypeId] [uniqueidentifier] NOT NULL,
	[TeacherId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](150) NOT NULL,
	[IsActive] [bit] NOT NULL,
 CONSTRAINT [PK_Contests] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER AUTHORIZATION ON [dbo].[Contests] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[ContestsTypes]    Script Date: 26.05.2014 19:06:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ContestsTypes](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](20) NOT NULL,
 CONSTRAINT [PK_ContestsTypes] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER AUTHORIZATION ON [dbo].[ContestsTypes] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[DevelopmentTools]    Script Date: 26.05.2014 19:06:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DevelopmentTools](
	[Id] [uniqueidentifier] NOT NULL,
	[ContestId] [uniqueidentifier] NULL,
	[Name] [nvarchar](150) NOT NULL,
	[CompileCommand] [nvarchar](256) NOT NULL,
	[CommandArgs] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_DevelopmentTools] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER AUTHORIZATION ON [dbo].[DevelopmentTools] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[ExceptionsLogs]    Script Date: 26.05.2014 19:06:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ExceptionsLogs](
	[Id] [uniqueidentifier] NOT NULL,
	[ContestId] [uniqueidentifier] NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[Text] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_ExceptionsLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER AUTHORIZATION ON [dbo].[ExceptionsLogs] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[PostedSolutions]    Script Date: 26.05.2014 19:06:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PostedSolutions](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[TaskId] [uniqueidentifier] NOT NULL,
	[DevToolId] [uniqueidentifier] NULL,
	[SolutionPath] [nvarchar](256) NOT NULL,
	[IsChecked] [bit] NOT NULL,
 CONSTRAINT [PK_PostedSolutions] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER AUTHORIZATION ON [dbo].[PostedSolutions] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[Tasks]    Script Date: 26.05.2014 19:06:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tasks](
	[Id] [uniqueidentifier] NOT NULL,
	[ContestId] [uniqueidentifier] NULL,
	[Name] [nvarchar](50) NOT NULL,
	[ExecutableName] [nvarchar](50) NOT NULL,
	[CheckerName] [nvarchar](50) NOT NULL,
	[TestsFolder] [nvarchar](256) NOT NULL,
	[TimeLimit] [int] NOT NULL,
	[Weight] [int] NOT NULL,
	[MaxSourceSize] [int] NOT NULL,
	[MaxMemorySize] [int] NOT NULL,
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER AUTHORIZATION ON [dbo].[Tasks] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[Teachers]    Script Date: 26.05.2014 19:06:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Teachers](
	[Id] [uniqueidentifier] NOT NULL,
	[TeacherName] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[Salt] [nvarchar](128) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[LastVisitDate] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Teachers] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER AUTHORIZATION ON [dbo].[Teachers] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserAttempts]    Script Date: 26.05.2014 19:06:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserAttempts](
	[Id] [uniqueidentifier] NOT NULL,
	[ContestId] [uniqueidentifier] NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[TaskId] [uniqueidentifier] NULL,
	[DevToolId] [uniqueidentifier] NULL,
	[SolutionPath] [nvarchar](256) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[Score] [int] NOT NULL,
	[FailedChecks] [int] NOT NULL,
	[FailedRuns] [int] NOT NULL,
 CONSTRAINT [PK_UserAttempts] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER AUTHORIZATION ON [dbo].[UserAttempts] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[UserInfos]    Script Date: 26.05.2014 19:06:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserInfos](
	[Id] [uniqueidentifier] NOT NULL,
	[School] [nvarchar](150) NOT NULL,
	[Grade] [int] NOT NULL,
	[HomeTown] [nvarchar](150) NOT NULL,
 CONSTRAINT [PK_UserInfos] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER AUTHORIZATION ON [dbo].[UserInfos] TO  SCHEMA OWNER 
GO
/****** Object:  Table [dbo].[Users]    Script Date: 26.05.2014 19:06:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[Id] [uniqueidentifier] NOT NULL,
	[UserInfoId] [uniqueidentifier] NULL,
	[Ip] [nvarchar](15) NOT NULL,
	[UserName] [nvarchar](128) NOT NULL,
	[Email] [nvarchar](128) NOT NULL,
	[Password] [nvarchar](128) NOT NULL,
	[Salt] [nvarchar](128) NOT NULL,
	[FirstName] [nvarchar](128) NOT NULL,
	[LastName] [nvarchar](128) NOT NULL,
	[MiddleName] [nvarchar](128) NOT NULL,
	[CreatedDate] [datetime2](7) NOT NULL,
	[LastVisitDate] [datetime2](7) NOT NULL,
	[IsDeleted] [bit] NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER AUTHORIZATION ON [dbo].[Users] TO  SCHEMA OWNER 
GO
INSERT [dbo].[ContestsTypes] ([Id], [Name]) VALUES (N'c1caf44e-687b-4768-910d-59d3005d1fa7', N'KYROV')
INSERT [dbo].[ContestsTypes] ([Id], [Name]) VALUES (N'f4ed477d-952b-42f2-8e11-82c7c8da951c', N'ACM')
ALTER TABLE [dbo].[Administrators] ADD  CONSTRAINT [DF_Administrators_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Contests] ADD  CONSTRAINT [DF_Contests_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ContestsTypes] ADD  CONSTRAINT [DF_ContestsTypes_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[DevelopmentTools] ADD  CONSTRAINT [DF_DevelopmentTools_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[ExceptionsLogs] ADD  CONSTRAINT [DF_ExceptionsLogs_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[PostedSolutions] ADD  CONSTRAINT [DF_PostedSolutions_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Tasks] ADD  CONSTRAINT [DF_Tasks_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Teachers] ADD  CONSTRAINT [DF_Teachers_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[UserAttempts] ADD  CONSTRAINT [DF_UserAttempts_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[UserInfos] ADD  CONSTRAINT [DF_UserInfos_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Users] ADD  CONSTRAINT [DF_Users_Id]  DEFAULT (newid()) FOR [Id]
GO
ALTER TABLE [dbo].[Contests]  WITH CHECK ADD  CONSTRAINT [FK_Contests_ContestsTypes] FOREIGN KEY([TypeId])
REFERENCES [dbo].[ContestsTypes] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Contests] CHECK CONSTRAINT [FK_Contests_ContestsTypes]
GO
ALTER TABLE [dbo].[Contests]  WITH CHECK ADD  CONSTRAINT [FK_Contests_Teachers] FOREIGN KEY([TeacherId])
REFERENCES [dbo].[Teachers] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Contests] CHECK CONSTRAINT [FK_Contests_Teachers]
GO
ALTER TABLE [dbo].[DevelopmentTools]  WITH CHECK ADD  CONSTRAINT [FK_DevelopmentTools_Contests] FOREIGN KEY([ContestId])
REFERENCES [dbo].[Contests] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[DevelopmentTools] CHECK CONSTRAINT [FK_DevelopmentTools_Contests]
GO
ALTER TABLE [dbo].[ExceptionsLogs]  WITH CHECK ADD  CONSTRAINT [FK_ExceptionsLogs_Contests] FOREIGN KEY([ContestId])
REFERENCES [dbo].[Contests] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[ExceptionsLogs] CHECK CONSTRAINT [FK_ExceptionsLogs_Contests]
GO
ALTER TABLE [dbo].[ExceptionsLogs]  WITH CHECK ADD  CONSTRAINT [FK_ExceptionsLogs_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[ExceptionsLogs] CHECK CONSTRAINT [FK_ExceptionsLogs_Users]
GO
ALTER TABLE [dbo].[PostedSolutions]  WITH CHECK ADD  CONSTRAINT [FK_PostedSolutions_DevelopmentTools] FOREIGN KEY([DevToolId])
REFERENCES [dbo].[DevelopmentTools] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[PostedSolutions] CHECK CONSTRAINT [FK_PostedSolutions_DevelopmentTools]
GO
ALTER TABLE [dbo].[PostedSolutions]  WITH CHECK ADD  CONSTRAINT [FK_PostedSolutions_Tasks] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Tasks] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PostedSolutions] CHECK CONSTRAINT [FK_PostedSolutions_Tasks]
GO
ALTER TABLE [dbo].[PostedSolutions]  WITH CHECK ADD  CONSTRAINT [FK_PostedSolutions_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PostedSolutions] CHECK CONSTRAINT [FK_PostedSolutions_Users]
GO
ALTER TABLE [dbo].[Tasks]  WITH CHECK ADD  CONSTRAINT [FK_Tasks_Contests] FOREIGN KEY([ContestId])
REFERENCES [dbo].[Contests] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Tasks] CHECK CONSTRAINT [FK_Tasks_Contests]
GO
ALTER TABLE [dbo].[UserAttempts]  WITH CHECK ADD  CONSTRAINT [FK_UserAttempts_Contests] FOREIGN KEY([ContestId])
REFERENCES [dbo].[Contests] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[UserAttempts] CHECK CONSTRAINT [FK_UserAttempts_Contests]
GO
ALTER TABLE [dbo].[UserAttempts]  WITH CHECK ADD  CONSTRAINT [FK_UserAttempts_DevelopmentTools] FOREIGN KEY([DevToolId])
REFERENCES [dbo].[DevelopmentTools] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[UserAttempts] CHECK CONSTRAINT [FK_UserAttempts_DevelopmentTools]
GO
ALTER TABLE [dbo].[UserAttempts]  WITH CHECK ADD  CONSTRAINT [FK_UserAttempts_Tasks] FOREIGN KEY([TaskId])
REFERENCES [dbo].[Tasks] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[UserAttempts] CHECK CONSTRAINT [FK_UserAttempts_Tasks]
GO
ALTER TABLE [dbo].[UserAttempts]  WITH CHECK ADD  CONSTRAINT [FK_UserAttempts_Users] FOREIGN KEY([UserId])
REFERENCES [dbo].[Users] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserAttempts] CHECK CONSTRAINT [FK_UserAttempts_Users]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_UserInfos] FOREIGN KEY([UserInfoId])
REFERENCES [dbo].[UserInfos] ([Id])
ON DELETE SET NULL
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK_Users_UserInfos]
GO
