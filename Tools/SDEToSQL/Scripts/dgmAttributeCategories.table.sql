IF OBJECT_ID('dbo.dgmAttributeCategories', 'U') IS NOT NULL
DROP TABLE [dbo].[dgmAttributeCategories]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[dgmAttributeCategories](
	[categoryID] [tinyint] NOT NULL,
	[categoryName] [nvarchar](50) NULL,
	[categoryDescription] [nvarchar](200) NULL,
 CONSTRAINT [dgmAttributeCategories_PK] PRIMARY KEY CLUSTERED 
(
	[categoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]