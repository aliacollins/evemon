IF OBJECT_ID('dbo.dgmTypeAttributes', 'U') IS NOT NULL
DROP TABLE [dbo].[dgmTypeAttributes]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[dgmTypeAttributes](
	[typeID] [int] NOT NULL,
	[attributeID] [smallint] NOT NULL,
	[valueInt] [int] NULL,
	[valueFloat] [float] NULL,
 CONSTRAINT [dgmTypeAttributes_PK] PRIMARY KEY CLUSTERED 
(
	[typeID] ASC,
	[attributeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]