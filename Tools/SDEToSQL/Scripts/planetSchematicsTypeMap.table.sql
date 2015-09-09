IF OBJECT_ID('dbo.planetSchematicsTypeMap', 'U') IS NOT NULL
DROP TABLE [dbo].[planetSchematicsTypeMap]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[planetSchematicsTypeMap](
	[schematicID] [smallint] NOT NULL,
	[typeID] [int] NOT NULL,
	[quantity] [smallint] NULL,
	[isInput] [bit] NULL,
 CONSTRAINT [planetSchematicsTypeMap_PK] PRIMARY KEY CLUSTERED 
(
	[schematicID] ASC,
	[typeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]