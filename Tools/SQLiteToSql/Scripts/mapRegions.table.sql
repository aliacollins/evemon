SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[mapRegions](
	[regionID] [int] NOT NULL,
	[regionName] [nvarchar](100) NULL,
	[x] [float] NULL,
	[y] [float] NULL,
	[z] [float] NULL,
	[xMin] [float] NULL,
	[xMax] [float] NULL,
	[yMin] [float] NULL,
	[yMax] [float] NULL,
	[zMin] [float] NULL,
	[zMax] [float] NULL,
	[factionID] [int] NULL,
	[radius] [float] NULL,
 CONSTRAINT [mapRegions_PK] PRIMARY KEY CLUSTERED 
(
	[regionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]