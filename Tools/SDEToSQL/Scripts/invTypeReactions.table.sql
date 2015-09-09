IF OBJECT_ID('dbo.invTypeReactions', 'U') IS NOT NULL
DROP TABLE [dbo].[invTypeReactions]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[invTypeReactions](
	[reactionTypeID] [int] NOT NULL,
	[input] [bit] NOT NULL,
	[typeID] [int] NOT NULL,
	[quantity] [smallint] NULL,
 CONSTRAINT [pk_invTypeReactions] PRIMARY KEY CLUSTERED 
(
	[reactionTypeID] ASC,
	[input] ASC,
	[typeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]