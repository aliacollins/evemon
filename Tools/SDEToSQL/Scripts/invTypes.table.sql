IF OBJECT_ID('dbo.invTypes', 'U') IS NOT NULL
DROP TABLE [dbo].[invTypes]; 

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[invTypes](
	[typeID] [int] NOT NULL,
	[groupID] [int] NULL,
	[typeName] [nvarchar](100) NULL,
	[description] [nvarchar](3000) NULL,
	[mass] [float] NULL,
	[volume] [float] NULL,
	[capacity] [float] NULL,
	[portionSize] [int] NULL,
	[raceID] [tinyint] NULL,
	[basePrice] [money] NULL,
	[published] [bit] NULL,
	[marketGroupID] [int] NULL,
	[chanceOfDuplicating] [float] NULL,
	[factionID] [int] NULL,
	[graphicID] [int] NULL,
	[iconID] [int] NULL,
	[radius] [float] NULL,
	[soundID] [int] NULL,
 CONSTRAINT [invTypes_PK] PRIMARY KEY CLUSTERED 
(
	[typeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
