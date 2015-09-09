IF OBJECT_ID('dbo.ramAssemblyLineStations', 'U') IS NOT NULL
DROP TABLE [dbo].[ramAssemblyLineStations]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ramAssemblyLineStations](
	[stationID] [int] NOT NULL,
	[assemblyLineTypeID] [tinyint] NOT NULL,
	[quantity] [tinyint] NULL,
	[stationTypeID] [int] NULL,
	[ownerID] [int] NULL,
	[solarSystemID] [int] NULL,
	[regionID] [int] NULL,
 CONSTRAINT [ramAssemblyLineStations_PK] PRIMARY KEY CLUSTERED 
(
	[stationID] ASC,
	[assemblyLineTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]