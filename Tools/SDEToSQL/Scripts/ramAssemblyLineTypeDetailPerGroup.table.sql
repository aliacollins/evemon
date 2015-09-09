IF OBJECT_ID('dbo.ramAssemblyLineTypeDetailPerGroup', 'U') IS NOT NULL
DROP TABLE [dbo].[ramAssemblyLineTypeDetailPerGroup]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ramAssemblyLineTypeDetailPerGroup](
	[assemblyLineTypeID] [tinyint] NOT NULL,
	[groupID] [int] NOT NULL,
	[timeMultiplier] [float] NULL,
	[materialMultiplier] [float] NULL,
	[costMultiplier] [float] NULL,
 CONSTRAINT [ramAssemblyLineTypeDetailPerGroup_PK] PRIMARY KEY CLUSTERED 
(
	[assemblyLineTypeID] ASC,
	[groupID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]