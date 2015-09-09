IF OBJECT_ID('dbo.ramAssemblyLineTypes', 'U') IS NOT NULL
DROP TABLE [dbo].[ramAssemblyLineTypes]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ramAssemblyLineTypes](
	[assemblyLineTypeID] [tinyint] NOT NULL,
	[assemblyLineTypeName] [nvarchar](100) NULL,
	[description] [nvarchar](1000) NULL,
	[baseTimeMultiplier] [float] NULL,
	[baseMaterialMultiplier] [float] NULL,
	[baseCostMultiplier] [float] NULL,
	[volume] [float] NULL,
	[activityID] [tinyint] NULL,
	[minCostPerHour] [float] NULL,
 CONSTRAINT [ramAssemblyLineTypes_PK] PRIMARY KEY CLUSTERED 
(
	[assemblyLineTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]