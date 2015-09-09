IF OBJECT_ID('dbo.dgmAttributeTypes', 'U') IS NOT NULL
DROP TABLE [dbo].[dgmAttributeTypes]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[dgmAttributeTypes](
	[attributeID] [smallint] NOT NULL,
	[attributeName] [varchar](100) NULL,
	[description] [varchar](1000) NULL,
	[iconID] [int] NULL,
	[defaultValue] [float] NULL,
	[published] [bit] NULL,
	[displayName] [varchar](100) NULL,
	[unitID] [tinyint] NULL,
	[stackable] [bit] NULL,
	[highIsGood] [bit] NULL,
	[categoryID] [tinyint] NULL,
 CONSTRAINT [dgmAttributeTypes_PK] PRIMARY KEY CLUSTERED 
(
	[attributeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF