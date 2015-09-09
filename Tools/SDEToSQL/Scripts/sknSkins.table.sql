IF OBJECT_ID('dbo.sknSkins', 'U') IS NOT NULL
DROP TABLE [dbo].[sknSkins]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[sknSkins](
	[skinID] [int] NOT NULL,
	[internalName] [nvarchar](100) NOT NULL,
	[skinMaterialID] [int] NULL,
	[typeID] [int] NULL,
	[allowCCPDevs] [bit] NOT NULL,
	[visibleSerenity] [bit] NOT NULL,
	[visibleTranquility] [bit] NOT NULL,
 CONSTRAINT [sknSkins_PK] PRIMARY KEY CLUSTERED 
(
	[skinID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF

ALTER TABLE [dbo].[sknSkins] ADD  DEFAULT ('') FOR [internalName]

ALTER TABLE [dbo].[sknSkins] ADD  DEFAULT ((0)) FOR [allowCCPDevs]

ALTER TABLE [dbo].[sknSkins] ADD  DEFAULT ((0)) FOR [visibleSerenity]

ALTER TABLE [dbo].[sknSkins] ADD  DEFAULT ((0)) FOR [visibleTranquility]