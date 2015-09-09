IF OBJECT_ID('dbo.crpNPCDivisions', 'U') IS NOT NULL
DROP TABLE [dbo].[crpNPCDivisions]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[crpNPCDivisions](
	[divisionID] [tinyint] NOT NULL,
	[divisionName] [nvarchar](100) NULL,
	[description] [nvarchar](1000) NULL,
	[leaderType] [nvarchar](100) NULL,
 CONSTRAINT [crpNPCDivisions_PK] PRIMARY KEY CLUSTERED 
(
	[divisionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]