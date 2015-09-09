IF OBJECT_ID('dbo.translationTables', 'U') IS NOT NULL
DROP TABLE [dbo].[translationTables]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[translationTables](
	[sourceTable] [nvarchar](200) NOT NULL,
	[destinationTable] [nvarchar](200) NULL,
	[translatedKey] [nvarchar](200) NOT NULL,
	[tcGroupID] [int] NULL,
	[tcID] [int] NULL,
 CONSTRAINT [translationTables_PK] PRIMARY KEY CLUSTERED 
(
	[sourceTable] ASC,
	[translatedKey] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]