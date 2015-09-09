IF OBJECT_ID('dbo.invGroups', 'U') IS NOT NULL
DROP TABLE [dbo].[invGroups]; 

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[invGroups](
	[groupID] [int] NOT NULL,
	[categoryID] [int] NULL,
	[groupName] [nvarchar](100) NULL,
	[description] [nvarchar](3000) NULL,
	[iconID] [int] NULL,
	[useBasePrice] [bit] NULL,
	[allowManufacture] [bit] NULL,
	[allowRecycler] [bit] NULL,
	[anchored] [bit] NULL,
	[anchorable] [bit] NULL,
	[fittableNonSingleton] [bit] NULL,
	[published] [bit] NULL,
 CONSTRAINT [invGroups_PK] PRIMARY KEY CLUSTERED 
(
	[groupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]