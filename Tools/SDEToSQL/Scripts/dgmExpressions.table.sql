IF OBJECT_ID('dbo.dgmExpressions', 'U') IS NOT NULL
DROP TABLE [dbo].[dgmExpressions]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

SET ANSI_PADDING ON

CREATE TABLE [dbo].[dgmExpressions](
	[expressionID] [int] NOT NULL,
	[operandID] [int] NULL,
	[arg1] [int] NULL,
	[arg2] [int] NULL,
	[expressionValue] [varchar](100) NULL,
	[description] [varchar](1000) NULL,
	[expressionName] [varchar](500) NULL,
	[expressionTypeID] [int] NULL,
	[expressionGroupID] [smallint] NULL,
	[expressionAttributeID] [smallint] NULL,
 CONSTRAINT [dgmExpressions_PK] PRIMARY KEY CLUSTERED 
(
	[expressionID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

SET ANSI_PADDING OFF