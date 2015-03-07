SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[mapCelestialStatistics](
	[celestialID] [int] NOT NULL,
	[temperature] [float] NULL,
	[spectralClass] [varchar](10) NULL,
	[luminosity] [float] NULL,
	[age] [float] NULL,
	[life] [float] NULL,
	[orbitRadius] [float] NULL,
	[eccentricity] [float] NULL,
	[massDust] [float] NULL,
	[massGas] [float] NULL,
	[fragmented] [bit] NULL,
	[density] [float] NULL,
	[surfaceGravity] [float] NULL,
	[escapeVelocity] [float] NULL,
	[orbitPeriod] [float] NULL,
	[rotationRate] [float] NULL,
	[locked] [bit] NULL,
	[pressure] [float] NULL,
	[radius] [float] NULL,
	[mass] [float] NULL,
 CONSTRAINT [mapCelestialStatistics_PK] PRIMARY KEY CLUSTERED 
(
	[celestialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF