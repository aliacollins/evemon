IF OBJECT_ID('dbo.invMarketGroups', 'U') IS NOT NULL
DROP TABLE [dbo].[invMarketGroups]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[invMarketGroups](
	[marketGroupID] [int] NOT NULL,
	[parentGroupID] [int] NULL,
	[marketGroupName] [nvarchar](100) NULL,
	[description] [nvarchar](3000) NULL,
	[iconID] [int] NULL,
	[hasTypes] [bit] NULL,
 CONSTRAINT [invMarketGroups_PK] PRIMARY KEY CLUSTERED 
(
	[marketGroupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]