IF OBJECT_ID('dbo.invMetaGroups', 'U') IS NOT NULL
DROP TABLE [dbo].[invMetaGroups]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[invMetaGroups](
	[metaGroupID] [smallint] NOT NULL,
	[metaGroupName] [nvarchar](100) NULL,
	[description] [nvarchar](1000) NULL,
	[iconID] [int] NULL,
 CONSTRAINT [invMetaGroups_PK] PRIMARY KEY CLUSTERED 
(
	[metaGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]