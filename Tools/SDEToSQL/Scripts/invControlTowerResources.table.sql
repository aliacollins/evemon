IF OBJECT_ID('dbo.invControlTowerResources', 'U') IS NOT NULL
DROP TABLE [dbo].[invControlTowerResources]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[invControlTowerResources](
	[controlTowerTypeID] [int] NOT NULL,
	[resourceTypeID] [int] NOT NULL,
	[purpose] [tinyint] NULL,
	[quantity] [int] NULL,
	[minSecurityLevel] [float] NULL,
	[factionID] [int] NULL,
 CONSTRAINT [invControlTowerResources_PK] PRIMARY KEY CLUSTERED 
(
	[controlTowerTypeID] ASC,
	[resourceTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]