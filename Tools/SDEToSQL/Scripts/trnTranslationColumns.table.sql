IF OBJECT_ID('dbo.trnTranslationColumns', 'U') IS NOT NULL
DROP TABLE [dbo].[trnTranslationColumns]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[trnTranslationColumns](
	[tcGroupID] [smallint] NULL,
	[tcID] [smallint] NOT NULL,
	[tableName] [nvarchar](256) NOT NULL,
	[columnName] [nvarchar](128) NOT NULL,
	[masterID] [nvarchar](128) NULL,
 CONSTRAINT [translationColumns_PK] PRIMARY KEY CLUSTERED 
(
	[tcID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]