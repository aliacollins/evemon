IF OBJECT_ID('dbo.invControlTowerResourcePurposes', 'U') IS NOT NULL
DROP TABLE [dbo].[invControlTowerResourcePurposes]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[invControlTowerResourcePurposes](
	[purpose] [tinyint] NOT NULL,
	[purposeText] [varchar](100) NULL,
 CONSTRAINT [invControlTowerResourcePurposes_PK] PRIMARY KEY CLUSTERED 
(
	[purpose] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF