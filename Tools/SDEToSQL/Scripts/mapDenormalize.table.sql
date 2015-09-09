SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[mapDenormalize](
	[itemID] [int] NOT NULL,
	[typeID] [int] NULL,
	[groupID] [int] NULL,
	[solarSystemID] [int] NULL,
	[constellationID] [int] NULL,
	[regionID] [int] NULL,
	[orbitID] [int] NULL,
	[x] [float] NULL,
	[y] [float] NULL,
	[z] [float] NULL,
	[radius] [float] NULL,
	[itemName] [nvarchar](100) NULL,
	[security] [float] NULL,
	[celestialIndex] [tinyint] NULL,
	[orbitIndex] [tinyint] NULL,
 CONSTRAINT [mapDenormalize_PK] PRIMARY KEY CLUSTERED 
(
	[itemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]