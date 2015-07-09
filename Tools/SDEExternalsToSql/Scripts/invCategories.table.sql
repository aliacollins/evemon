SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[invCategories](
	[categoryID] [int] NOT NULL,
	[categoryName] [nvarchar](100) NULL,
	[description] [nvarchar](3000) NULL,
	[iconID] [int] NULL,
	[published] [bit] NULL,
 CONSTRAINT [invCategories_PK] PRIMARY KEY CLUSTERED 
(
	[categoryID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]