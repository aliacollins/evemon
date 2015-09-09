IF OBJECT_ID('dbo.chrFactions', 'U') IS NOT NULL
DROP TABLE [dbo].[chrFactions]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[chrFactions](
	[factionID] [int] NOT NULL,
	[factionName] [varchar](100) NULL,
	[description] [varchar](1000) NULL,
	[raceIDs] [int] NULL,
	[solarSystemID] [int] NULL,
	[corporationID] [int] NULL,
	[sizeFactor] [float] NULL,
	[stationCount] [smallint] NULL,
	[stationSystemCount] [smallint] NULL,
	[militiaCorporationID] [int] NULL,
	[iconID] [int] NULL,
 CONSTRAINT [chrFactions_PK] PRIMARY KEY CLUSTERED 
(
	[factionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF