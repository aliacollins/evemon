IF OBJECT_ID('dbo.mapLocationWormholeClasses', 'U') IS NOT NULL
DROP TABLE [dbo].[mapLocationWormholeClasses]; 

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[mapLocationWormholeClasses](
	[locationID] [int] NOT NULL,
	[wormholeClassID] [tinyint] NULL,
 CONSTRAINT [mapLocationWormholeClasses_PK] PRIMARY KEY CLUSTERED 
(
	[locationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]