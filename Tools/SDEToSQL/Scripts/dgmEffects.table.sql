IF OBJECT_ID('dbo.dgmEffects', 'U') IS NOT NULL
DROP TABLE [dbo].[dgmEffects]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[dgmEffects](
	[effectID] [smallint] NOT NULL,
	[effectName] [varchar](400) NULL,
	[effectCategory] [smallint] NULL,
	[preExpression] [int] NULL,
	[postExpression] [int] NULL,
	[description] [varchar](1000) NULL,
	[guid] [varchar](60) NULL,
	[iconID] [int] NULL,
	[isOffensive] [bit] NULL,
	[isAssistance] [bit] NULL,
	[durationAttributeID] [smallint] NULL,
	[trackingSpeedAttributeID] [smallint] NULL,
	[dischargeAttributeID] [smallint] NULL,
	[rangeAttributeID] [smallint] NULL,
	[falloffAttributeID] [smallint] NULL,
	[disallowAutoRepeat] [bit] NULL,
	[published] [bit] NULL,
	[displayName] [varchar](100) NULL,
	[isWarpSafe] [bit] NULL,
	[rangeChance] [bit] NULL,
	[electronicChance] [bit] NULL,
	[propulsionChance] [bit] NULL,
	[distribution] [tinyint] NULL,
	[sfxName] [varchar](20) NULL,
	[npcUsageChanceAttributeID] [smallint] NULL,
	[npcActivationChanceAttributeID] [smallint] NULL,
	[fittingUsageChanceAttributeID] [smallint] NULL,
	[modifierInfo] [varchar](max) NULL,
 CONSTRAINT [dgmEffects_PK] PRIMARY KEY CLUSTERED 
(
	[effectID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

SET ANSI_PADDING OFF