IF OBJECT_ID('dbo.ramAssemblyLineTypeDetailPerCategory', 'U') IS NOT NULL
DROP TABLE [dbo].[ramAssemblyLineTypeDetailPerCategory]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ramAssemblyLineTypeDetailPerCategory](
	[assemblyLineTypeID] [tinyint] NOT NULL,
	[categoryID] [int] NOT NULL,
	[timeMultiplier] [float] NULL,
	[materialMultiplier] [float] NULL,
	[costMultiplier] [float] NULL,
 CONSTRAINT [ramAssemblyLineTypeDetailPerCategory_PK] PRIMARY KEY CLUSTERED 
(
	[assemblyLineTypeID] ASC,
	[categoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]