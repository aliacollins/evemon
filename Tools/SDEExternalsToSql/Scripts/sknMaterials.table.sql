SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[sknMaterials](
	[skinMaterialID] [int] NOT NULL,
	[materialSetID] [int] NOT NULL,
	[displayNameID] [int] NOT NULL,
	-- Obsolete columns since Galatea 1.0
	[material] [nvarchar](255) NOT NULL,
	[colorHull] [nvarchar](6) NULL,
	[colorWindow] [nvarchar](6) NULL,
	[colorPrimary] [nvarchar](6) NULL,
	[colorSecondary] [nvarchar](6) NULL,
 CONSTRAINT [sknMaterials_PK] PRIMARY KEY CLUSTERED 
(
	[skinMaterialID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_PADDING OFF

ALTER TABLE [dbo].[sknMaterials] ADD  DEFAULT ('') FOR [material]

ALTER TABLE [dbo].[sknMaterials] ADD  DEFAULT ((0)) FOR [materialSetID]

ALTER TABLE [dbo].[sknMaterials] ADD  DEFAULT ((0)) FOR [displayNameID]
