IF OBJECT_ID('dbo.staStations', 'U') IS NOT NULL
DROP TABLE [dbo].[staStations]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[staStations](
	[stationID] [int] NOT NULL,
	[security] [smallint] NULL,
	[dockingCostPerVolume] [float] NULL,
	[maxShipVolumeDockable] [float] NULL,
	[officeRentalCost] [int] NULL,
	[operationID] [tinyint] NULL,
	[stationTypeID] [int] NULL,
	[corporationID] [int] NULL,
	[solarSystemID] [int] NULL,
	[constellationID] [int] NULL,
	[regionID] [int] NULL,
	[stationName] [nvarchar](100) NULL,
	[x] [float] NULL,
	[y] [float] NULL,
	[z] [float] NULL,
	[reprocessingEfficiency] [float] NULL,
	[reprocessingStationsTake] [float] NULL,
	[reprocessingHangarFlag] [tinyint] NULL,
 CONSTRAINT [staStations_PK] PRIMARY KEY CLUSTERED 
(
	[stationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
