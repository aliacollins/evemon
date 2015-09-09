IF OBJECT_ID('dbo.agtAgents', 'U') IS NOT NULL
DROP TABLE [dbo].[agtAgents]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[agtAgents](
	[agentID] [int] NOT NULL,
	[divisionID] [tinyint] NULL,
	[corporationID] [int] NULL,
	[locationID] [int] NULL,
	[level] [tinyint] NULL,
	[quality] [smallint] NULL,
	[agentTypeID] [int] NULL,
	[isLocator] [bit] NULL,
 CONSTRAINT [agtAgents_PK] PRIMARY KEY CLUSTERED 
(
	[agentID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]