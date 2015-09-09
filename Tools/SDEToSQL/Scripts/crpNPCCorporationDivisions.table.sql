IF OBJECT_ID('dbo.crpNPCCorporationDivisions', 'U') IS NOT NULL
DROP TABLE [dbo].[crpNPCCorporationDivisions]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[crpNPCCorporationDivisions](
	[corporationID] [int] NOT NULL,
	[divisionID] [tinyint] NOT NULL,
	[size] [tinyint] NULL,
 CONSTRAINT [crpNPCCorporationDivisions_PK] PRIMARY KEY CLUSTERED 
(
	[corporationID] ASC,
	[divisionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]