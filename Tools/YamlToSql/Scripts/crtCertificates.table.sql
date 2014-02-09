SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[crtCertificates](
	[certificateID] [int] NOT NULL,
	[groupID] [smallint] NULL,
	[classID] [int] NULL,
	[grade] [tinyint] NULL,
	[corpID] [int] NULL,
	[iconID] [int] NULL,
	[description] [nvarchar](500) NULL,
 CONSTRAINT [crtCertificates_PK] PRIMARY KEY CLUSTERED 
(
	[certificateID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]