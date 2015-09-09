IF OBJECT_ID('dbo.invBlueprintTypes', 'U') IS NOT NULL
DROP TABLE [dbo].[invBlueprintTypes]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[invBlueprintTypes](
	[blueprintTypeID] [int] NOT NULL,
	[parentBlueprintTypeID] [int] NULL,
	[productTypeID] [int] NULL,
	[productionTime] [int] NULL,
	[techLevel] [smallint] NULL,
	[researchProductivityTime] [int] NULL,
	[researchMaterialTime] [int] NULL,
	[researchCopyTime] [int] NULL,
	[researchTechTime] [int] NULL,
	[duplicatingTime] [int] NULL,
	[reverseEngineeringTime] [int] NULL,
	[inventionTime] [int] NULL,
	[productivityModifier] [int] NULL,
	[materialModifier] [smallint] NULL,
	[wasteFactor] [smallint] NULL,
	[maxProductionLimit] [int] NULL,
 CONSTRAINT [invBlueprintTypes_PK] PRIMARY KEY CLUSTERED 
(
	[blueprintTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]