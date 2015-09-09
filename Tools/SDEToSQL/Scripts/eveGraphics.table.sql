IF OBJECT_ID('dbo.eveGraphics', 'U') IS NOT NULL
DROP TABLE [dbo].[eveGraphics]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[eveGraphics](
	[graphicID] [int] NOT NULL,
	[graphicFile] [varchar](500) NOT NULL,
	[description] [nvarchar](max) NOT NULL,
	[obsolete] [bit] NOT NULL,
	[graphicType] [varchar](100) NULL,
	[collidable] [bit] NULL,
	[directoryID] [int] NULL,
	[graphicName] [nvarchar](64) NOT NULL,
	[gfxRaceID] [varchar](255) NULL,
	[colorScheme] [varchar](255) NULL,
	[sofHullName] [varchar](64) NULL,
 CONSTRAINT [eveGraphics_PK] PRIMARY KEY CLUSTERED 
(
	[graphicID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


SET ANSI_PADDING OFF

ALTER TABLE [dbo].[eveGraphics] ADD  DEFAULT ('') FOR [graphicFile]

ALTER TABLE [dbo].[eveGraphics] ADD  DEFAULT ('') FOR [description]

ALTER TABLE [dbo].[eveGraphics] ADD  DEFAULT ((0)) FOR [obsolete]

ALTER TABLE [dbo].[eveGraphics] ADD  DEFAULT ('') FOR [graphicName]