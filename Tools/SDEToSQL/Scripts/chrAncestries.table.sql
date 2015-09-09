IF OBJECT_ID('dbo.chrAncestries', 'U') IS NOT NULL
DROP TABLE [dbo].[chrAncestries]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[chrAncestries](
	[ancestryID] [tinyint] NOT NULL,
	[ancestryName] [nvarchar](100) NULL,
	[bloodlineID] [tinyint] NULL,
	[description] [nvarchar](1000) NULL,
	[perception] [tinyint] NULL,
	[willpower] [tinyint] NULL,
	[charisma] [tinyint] NULL,
	[memory] [tinyint] NULL,
	[intelligence] [tinyint] NULL,
	[iconID] [int] NULL,
	[shortDescription] [nvarchar](500) NULL,
 CONSTRAINT [chrAncestries_PK] PRIMARY KEY CLUSTERED 
(
	[ancestryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]