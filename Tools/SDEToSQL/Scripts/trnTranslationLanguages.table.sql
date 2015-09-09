IF OBJECT_ID('dbo.trnTranslationLanguages', 'U') IS NOT NULL
DROP TABLE [dbo].[trnTranslationLanguages]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[trnTranslationLanguages](
	[numericLanguageID] [int] NOT NULL,
	[languageID] [varchar](50) NULL,
	[languageName] [nvarchar](200) NULL,
 CONSTRAINT [trnTranslationLanguages_PK] PRIMARY KEY CLUSTERED 
(
	[numericLanguageID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF