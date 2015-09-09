IF OBJECT_ID('dbo.invPositions', 'U') IS NOT NULL
DROP TABLE [dbo].[invPositions]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[invPositions](
	[itemID] [bigint] NOT NULL,
	[x] [float] NOT NULL DEFAULT ((0.0)),
	[y] [float] NOT NULL DEFAULT ((0.0)),
	[z] [float] NOT NULL DEFAULT ((0.0)),
	[yaw] [real] NULL,
	[pitch] [real] NULL,
	[roll] [real] NULL,
 CONSTRAINT [invPositions_PK] PRIMARY KEY CLUSTERED 
(
	[itemID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]