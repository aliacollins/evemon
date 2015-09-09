IF OBJECT_ID('dbo.staOperationServices', 'U') IS NOT NULL
DROP TABLE [dbo].[staOperationServices]

SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[staOperationServices](
	[operationID] [tinyint] NOT NULL,
	[serviceID] [int] NOT NULL,
 CONSTRAINT [staOperationServices_PK] PRIMARY KEY CLUSTERED 
(
	[operationID] ASC,
	[serviceID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]