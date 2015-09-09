IF OBJECT_ID('dbo.staStationTypes', 'U') IS NOT NULL
DROP TABLE [dbo].[staStationTypes]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[staStationTypes](
	[stationTypeID] [int] NOT NULL,
	[dockEntryX] [float] NULL,
	[dockEntryY] [float] NULL,
	[dockEntryZ] [float] NULL,
	[dockOrientationX] [float] NULL,
	[dockOrientationY] [float] NULL,
	[dockOrientationZ] [float] NULL,
	[operationID] [tinyint] NULL,
	[officeSlots] [tinyint] NULL,
	[reprocessingEfficiency] [float] NULL,
	[conquerable] [bit] NULL,
 CONSTRAINT [stationTypes_PK] PRIMARY KEY CLUSTERED 
(
	[stationTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]