IF OBJECT_ID('dbo.chrAttributes', 'U') IS NOT NULL
DROP TABLE [dbo].[chrAttributes]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[chrAttributes](
	[attributeID] [tinyint] NOT NULL,
	[attributeName] [varchar](100) NULL,
	[description] [varchar](1000) NULL,
	[iconID] [int] NULL,
	[shortDescription] [nvarchar](500) NULL,
	[notes] [nvarchar](500) NULL,
 CONSTRAINT [chrAttributes_PK] PRIMARY KEY CLUSTERED 
(
	[attributeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF