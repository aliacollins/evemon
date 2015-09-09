IF OBJECT_ID('dbo.crpNPCCorporations', 'U') IS NOT NULL
DROP TABLE [dbo].[crpNPCCorporations]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[crpNPCCorporations](
	[corporationID] [int] NOT NULL,
	[size] [char](1) NULL,
	[extent] [char](1) NULL,
	[solarSystemID] [int] NULL,
	[investorID1] [int] NULL,
	[investorShares1] [tinyint] NULL,
	[investorID2] [int] NULL,
	[investorShares2] [tinyint] NULL,
	[investorID3] [int] NULL,
	[investorShares3] [tinyint] NULL,
	[investorID4] [int] NULL,
	[investorShares4] [tinyint] NULL,
	[friendID] [int] NULL,
	[enemyID] [int] NULL,
	[publicShares] [bigint] NULL,
	[initialPrice] [int] NULL,
	[minSecurity] [float] NULL,
	[scattered] [bit] NULL,
	[fringe] [tinyint] NULL,
	[corridor] [tinyint] NULL,
	[hub] [tinyint] NULL,
	[border] [tinyint] NULL,
	[factionID] [int] NULL,
	[sizeFactor] [float] NULL,
	[stationCount] [smallint] NULL,
	[stationSystemCount] [smallint] NULL,
	[description] [nvarchar](4000) NULL,
	[iconID] [int] NULL,
 CONSTRAINT [crpNPCCorporations_PK] PRIMARY KEY CLUSTERED 
(
	[corporationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF