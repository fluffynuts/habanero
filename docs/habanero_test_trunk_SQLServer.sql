USE [master]
GO
/****** Object:  Database [habanero_test_trunk]    Script Date: 07/08/2009 15:04:05 ******/
CREATE DATABASE [habanero_test_trunk] 
GO
USE [habanero_test_trunk]
GO
/****** Object:  Table [dbo].[tbPersonTable]    Script Date: 07/08/2009 15:04:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[tbPersonTable](
	[PersonID] [uniqueidentifier] NOT NULL,
	[Surname] [nchar](255) NOT NULL,
	[FirstName] [nchar](255) NULL,
 CONSTRAINT [PK_tbPersonTable] PRIMARY KEY CLUSTERED 
(
	[PersonID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT INTO [dbo].[tbPersonTable]
           ([PersonID]
           ,[Surname]
           ,[FirstName])
     VALUES
           ('C5DBF96C-C16C-433D-BE56-A8F9EB94118E','Test1','TestName1'),
           ('A0A769A0-017E-498D-8082-F3F2541B04E3','Test2',null)
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MyBO](
	[MyBoID] [uniqueidentifier] NOT NULL,
	[ByteArrayProp] [image] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

