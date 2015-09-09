IF OBJECT_ID('dbo.planetSchematicsPinMap', 'U') IS NOT NULL
DROP TABLE [dbo].[planetSchematicsPinMap]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[planetSchematicsPinMap](
	[schematicID] [smallint] NOT NULL,
	[pinTypeID] [int] NOT NULL,
 CONSTRAINT [planetSchematicsPinMap_PK] PRIMARY KEY CLUSTERED 
(
	[schematicID] ASC,
	[pinTypeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]