IF OBJECT_ID('dbo.warCombatZones', 'U') IS NOT NULL
DROP TABLE [dbo].[warCombatZones]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[warCombatZones](
	[combatZoneID] [int] NOT NULL DEFAULT ((-1)),
	[combatZoneName] [nvarchar](100) NULL,
	[factionID] [int] NULL,
	[centerSystemID] [int] NULL,
	[description] [nvarchar](500) NULL,
 CONSTRAINT [combatZones_PK] PRIMARY KEY CLUSTERED 
(
	[combatZoneID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]