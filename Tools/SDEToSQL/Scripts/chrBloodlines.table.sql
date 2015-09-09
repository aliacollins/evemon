IF OBJECT_ID('dbo.chrBloodlines', 'U') IS NOT NULL
DROP TABLE [dbo].[chrBloodlines]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[chrBloodlines](
	[bloodlineID] [tinyint] NOT NULL,
	[bloodlineName] [nvarchar](100) NULL,
	[raceID] [tinyint] NULL,
	[description] [nvarchar](1000) NULL,
	[maleDescription] [nvarchar](1000) NULL,
	[femaleDescription] [nvarchar](1000) NULL,
	[shipTypeID] [int] NULL,
	[corporationID] [int] NULL,
	[perception] [tinyint] NULL,
	[willpower] [tinyint] NULL,
	[charisma] [tinyint] NULL,
	[memory] [tinyint] NULL,
	[intelligence] [tinyint] NULL,
	[iconID] [int] NULL,
	[shortDescription] [nvarchar](500) NULL,
	[shortMaleDescription] [nvarchar](500) NULL,
	[shortFemaleDescription] [nvarchar](500) NULL,
 CONSTRAINT [chrBloodlines_PK] PRIMARY KEY CLUSTERED 
(
	[bloodlineID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]