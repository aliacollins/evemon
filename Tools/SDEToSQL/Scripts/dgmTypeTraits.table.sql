IF OBJECT_ID('dbo.dgmTypeTraits', 'U') IS NOT NULL
DROP TABLE [dbo].[dgmTypeTraits]; 

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[dgmTypeTraits](
	[typeID] [int] NOT NULL,
	[parentTypeID] [int] NOT NULL,
	[traitID] [int] NOT NULL,
	[bonus] [float] NULL,
 CONSTRAINT [dgmTypeTraits_PK] PRIMARY KEY CLUSTERED 
(
	[typeID] ASC,
	[parentTypeID] ASC,
	[traitID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]