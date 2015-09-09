IF OBJECT_ID('dbo.ramTypeRequirements', 'U') IS NOT NULL
DROP TABLE [dbo].[ramTypeRequirements]; 

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ramTypeRequirements](
	[typeID] [int] NOT NULL,
	[activityID] [tinyint] NOT NULL,
	[requiredTypeID] [int] NOT NULL,
	[quantity] [int] NULL,
	[level] [int] NULL,
	[damagePerJob] [float] NULL,
	[recycle] [bit] NULL,
	[raceID] [int] NULL,
	[probability] [float] NULL,
	[consume] [bit] NULL,
 CONSTRAINT [ramTypeRequirements_PK] PRIMARY KEY CLUSTERED 
(
	[typeID] ASC,
	[activityID] ASC,
	[requiredTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
