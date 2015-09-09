IF OBJECT_ID('dbo.mapUniverse', 'U') IS NOT NULL
DROP TABLE [dbo].[mapUniverse]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[mapUniverse](
	[universeID] [int] NOT NULL,
	[universeName] [varchar](100) NULL,
	[x] [float] NULL,
	[y] [float] NULL,
	[z] [float] NULL,
	[xMin] [float] NULL,
	[xMax] [float] NULL,
	[yMin] [float] NULL,
	[yMax] [float] NULL,
	[zMin] [float] NULL,
	[zMax] [float] NULL,
	[radius] [float] NULL,
 CONSTRAINT [mapUniverse_PK] PRIMARY KEY CLUSTERED 
(
	[universeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF