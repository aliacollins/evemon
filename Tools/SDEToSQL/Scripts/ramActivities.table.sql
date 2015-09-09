IF OBJECT_ID('dbo.ramActivities', 'U') IS NOT NULL
DROP TABLE [dbo].[ramActivities]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[ramActivities](
	[activityID] [tinyint] NOT NULL,
	[activityName] [nvarchar](100) NULL,
	[iconNo] [varchar](5) NULL,
	[description] [nvarchar](1000) NULL,
	[published] [bit] NULL,
 CONSTRAINT [ramActivities_PK] PRIMARY KEY CLUSTERED 
(
	[activityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF