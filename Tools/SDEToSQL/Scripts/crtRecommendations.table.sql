IF OBJECT_ID('dbo.crtRecommendations', 'U') IS NOT NULL
DROP TABLE [dbo].[crtRecommendations]; 

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[crtRecommendations](
	[recommendationID] [int] IDENTITY NOT NULL,
	[shipTypeID] [int] NULL,
	[certificateID] [int] NULL,
	[recommendationLevel] [tinyint] NOT NULL,
 CONSTRAINT [crtRecommendations_PK] PRIMARY KEY CLUSTERED 
(
	[recommendationID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

ALTER TABLE [dbo].[crtRecommendations] ADD  DEFAULT ((0)) FOR [recommendationLevel]