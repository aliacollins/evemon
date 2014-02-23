SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[mapConstellationJumps](
	[fromRegionID] [int] NULL,
	[fromConstellationID] [int] NOT NULL,
	[toConstellationID] [int] NOT NULL,
	[toRegionID] [int] NULL,
 CONSTRAINT [mapConstellationJumps_PK] PRIMARY KEY CLUSTERED 
(
	[fromConstellationID] ASC,
	[toConstellationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]