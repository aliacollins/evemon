IF OBJECT_ID('dbo.crpActivities', 'U') IS NOT NULL
DROP TABLE [dbo].[crpActivities]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[crpActivities](
	[activityID] [tinyint] NOT NULL,
	[activityName] [nvarchar](100) NULL,
	[description] [nvarchar](1000) NULL,
 CONSTRAINT [crpActivities_PK] PRIMARY KEY CLUSTERED 
(
	[activityID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]