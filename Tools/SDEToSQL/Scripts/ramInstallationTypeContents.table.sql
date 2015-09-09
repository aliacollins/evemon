IF OBJECT_ID('dbo.ramInstallationTypeContents', 'U') IS NOT NULL
DROP TABLE [dbo].[ramInstallationTypeContents]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[ramInstallationTypeContents](
	[installationTypeID] [int] NOT NULL,
	[assemblyLineTypeID] [tinyint] NOT NULL,
	[quantity] [tinyint] NULL,
 CONSTRAINT [ramInstallationTypeContents_PK] PRIMARY KEY CLUSTERED 
(
	[installationTypeID] ASC,
	[assemblyLineTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]