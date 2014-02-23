SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[mapSolarSystems](
	[regionID] [int] NULL,
	[constellationID] [int] NULL,
	[solarSystemID] [int] NOT NULL,
	[solarSystemName] [nvarchar](100) NULL,
	[x] [float] NULL,
	[y] [float] NULL,
	[z] [float] NULL,
	[xMin] [float] NULL,
	[xMax] [float] NULL,
	[yMin] [float] NULL,
	[yMax] [float] NULL,
	[zMin] [float] NULL,
	[zMax] [float] NULL,
	[luminosity] [float] NULL,
	[border] [bit] NULL,
	[fringe] [bit] NULL,
	[corridor] [bit] NULL,
	[hub] [bit] NULL,
	[international] [bit] NULL,
	[regional] [bit] NULL,
	[constellation] [bit] NULL,
	[security] [float] NULL,
	[factionID] [int] NULL,
	[radius] [float] NULL,
	[sunTypeID] [int] NULL,
	[securityClass] [varchar](2) NULL,
 CONSTRAINT [mapSolarSystems_PK] PRIMARY KEY CLUSTERED 
(
	[solarSystemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]