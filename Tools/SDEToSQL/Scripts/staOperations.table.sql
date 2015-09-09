IF OBJECT_ID('dbo.staOperations', 'U') IS NOT NULL
DROP TABLE [dbo].[staOperations]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[staOperations](
	[activityID] [tinyint] NULL,
	[operationID] [tinyint] NOT NULL,
	[operationName] [nvarchar](100) NULL,
	[description] [nvarchar](1000) NULL,
	[fringe] [tinyint] NULL,
	[corridor] [tinyint] NULL,
	[hub] [tinyint] NULL,
	[border] [tinyint] NULL,
	[ratio] [tinyint] NULL,
	[caldariStationTypeID] [int] NULL,
	[minmatarStationTypeID] [int] NULL,
	[amarrStationTypeID] [int] NULL,
	[gallenteStationTypeID] [int] NULL,
	[joveStationTypeID] [int] NULL,
 CONSTRAINT [staOperations_PK] PRIMARY KEY CLUSTERED 
(
	[operationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]