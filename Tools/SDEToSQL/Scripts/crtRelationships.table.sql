SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[crtRelationships](
	[relationshipID] [int] IDENTITY NOT NULL,
	[parentID] [int] NULL,
	[parentTypeID] [int] NULL,
	[parentLevel] [tinyint] NULL,
	[childID] [int] NULL,
	[grade] [tinyint] NULL,
 CONSTRAINT [crtRelationships_relationship] PRIMARY KEY CLUSTERED 
(
	[relationshipID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]