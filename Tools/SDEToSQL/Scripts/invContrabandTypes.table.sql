IF OBJECT_ID('dbo.invContrabandTypes', 'U') IS NOT NULL
DROP TABLE [dbo].[invContrabandTypes]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[invContrabandTypes](
	[factionID] [int] NOT NULL,
	[typeID] [int] NOT NULL,
	[standingLoss] [float] NULL,
	[confiscateMinSec] [float] NULL,
	[fineByValue] [float] NULL,
	[attackMinSec] [float] NULL,
 CONSTRAINT [invContrabandTypes_PK] PRIMARY KEY CLUSTERED 
(
	[factionID] ASC,
	[typeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]