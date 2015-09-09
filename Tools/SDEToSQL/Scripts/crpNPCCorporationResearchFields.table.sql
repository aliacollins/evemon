IF OBJECT_ID('dbo.crpNPCCorporationResearchFields', 'U') IS NOT NULL
DROP TABLE [dbo].[crpNPCCorporationResearchFields]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[crpNPCCorporationResearchFields](
	[skillID] [int] NOT NULL,
	[corporationID] [int] NOT NULL,
 CONSTRAINT [crpNPCCorporationResearchFields_PK] PRIMARY KEY CLUSTERED 
(
	[skillID] ASC,
	[corporationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]