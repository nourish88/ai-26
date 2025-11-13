USE [DATA_AUDIT]
GO

/****** Object:  Table [dbo].[AuditLogs]    Script Date: 10/23/2020 5:26:07 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [AuditLogs](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[EventTime] [datetime] NOT NULL,
	[EventType] [varchar](50) NOT NULL,
	[Pk1] [bigint] NULL,
	[Pk2] [bigint] NULL,
	[PkGuid] [uniqueidentifier] NULL,
	[Data] [nvarchar](max) NULL,
	[Database] [nvarchar](max) NULL,
	[Schema] [nvarchar](100) NULL,
	[Table] [nvarchar](100) NULL,
	[User] [nvarchar](100) NULL,
 CONSTRAINT [PK_AuditLogs] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


