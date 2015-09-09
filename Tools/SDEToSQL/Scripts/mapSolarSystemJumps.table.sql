IF OBJECT_ID('dbo.mapSolarSystemJumps', 'U') IS NOT NULL
DROP TABLE [dbo].[mapSolarSystemJumps]; 

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[mapSolarSystemJumps](
	[fromRegionID] [int] NULL,
	[fromConstellationID] [int] NULL,
	[fromSolarSystemID] [int] NOT NULL,
	[toSolarSystemID] [int] NOT NULL,
	[toConstellationID] [int] NULL,
	[toRegionID] [int] NULL,
 CONSTRAINT [mapSolarSystemJumps_PK] PRIMARY KEY CLUSTERED 
(
	[fromSolarSystemID] ASC,
	[toSolarSystemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]