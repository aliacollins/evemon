IF OBJECT_ID('dbo.mapLandmarks', 'U') IS NOT NULL
DROP TABLE [dbo].[mapLandmarks]; 

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[mapLandmarks](
	[landmarkID] [smallint] NOT NULL,
	[landmarkName] [varchar](100) NULL,
	[description] [varchar](7000) NULL,
	[locationID] [int] NULL,
	[x] [float] NULL,
	[y] [float] NULL,
	[z] [float] NULL,
	[radius] [float] NULL,
	[iconID] [int] NULL,
	[importance] [tinyint] NULL,
 CONSTRAINT [mapLandmarks_PK] PRIMARY KEY CLUSTERED 
(
	[landmarkID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF