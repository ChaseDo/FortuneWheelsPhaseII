USE [RotaryDrawDB2]
GO

/****** Object:  Table [dbo].[Awards]    Script Date: 02/24/2014 09:10:14 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Awards](
	[AwardID] [int] NOT NULL,
	[AwardName] [nvarchar](50) NOT NULL,
	[Angle] [int] NOT NULL,
	[Rate] [float] NOT NULL,
	[TotalCount] [int] NOT NULL,
	[SurplusCount] [int] NOT NULL,
 CONSTRAINT [PK_Awards] PRIMARY KEY CLUSTERED 
(
	[AwardID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[CardCodes](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[AwardID] [int] NOT NULL,
	[CardCode] [nvarchar](255) NOT NULL,
	[Used] [bit] NOT NULL,
 CONSTRAINT [PK_CardCodes] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[CardCodes]  WITH CHECK ADD  CONSTRAINT [FK_CardCodes_Awards] FOREIGN KEY([AwardID])
REFERENCES [dbo].[Awards] ([AwardID])
GO

ALTER TABLE [dbo].[CardCodes] CHECK CONSTRAINT [FK_CardCodes_Awards]
GO

ALTER TABLE [dbo].[CardCodes] ADD  CONSTRAINT [DF_CardCodes_Used]  DEFAULT ((0)) FOR [Used]
GO


CREATE TABLE [dbo].[DownloadHistory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CellPhoneNo] [nvarchar](50) NOT NULL,
	[Game1] [bit] NOT NULL,
	[Game1Time] [datetime] NULL,
	[Game2] [bit] NOT NULL,
	[Game2Time] [datetime] NULL,
 CONSTRAINT [PK_DownloadHistory] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

CREATE TABLE [dbo].[LotteryHistory](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CellPhoneNo] [nvarchar](50) NOT NULL,
	[LotteryTime] [datetime] NOT NULL,
	[AwardID] [int] NOT NULL,
	[CardCode] [nvarchar](255) NULL,
 CONSTRAINT [PK_WinInfo] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[LotteryHistory]  WITH CHECK ADD  CONSTRAINT [FK_LotteryHistory_Awards] FOREIGN KEY([AwardID])
REFERENCES [dbo].[Awards] ([AwardID])
GO

ALTER TABLE [dbo].[LotteryHistory] CHECK CONSTRAINT [FK_LotteryHistory_Awards]
GO


CREATE TABLE [dbo].[Players](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CellPhoneNo] [nvarchar](50) NOT NULL,
	[LotteryCount] [int] NOT NULL,
	[SurplusCount] [int] NOT NULL,
	[LastLoginTime] [datetime] NOT NULL,
 CONSTRAINT [PK_Players] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

Create PROCEDURE [dbo].[spDownloadGame1]
        -- Add the parameters for the stored procedure here
        @CellPhoneNo        nvarchar(50)='' 
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        SET NOCOUNT ON;

    -- Insert statements for procedure here
    IF EXISTS (SELECT * FROM dbo.Players WHERE CellPhoneNo = @CellPhoneNo)
    AND (SELECT [Game1] FROM [dbo].[DownloadHistory] WHERE CellPhoneNo = @CellPhoneNo) = 0
        BEGIN
			UPDATE		[dbo].[Players]
				SET		[LotteryCount] = [LotteryCount] + 1
				WHERE   [CellPhoneNo] = @CellPhoneNo   
					   
			UPDATE		[dbo].[DownloadHistory]
				SET     [Game1] = 1,
						[Game1Time] = GETDATE()
				WHERE   [CellPhoneNo] = @CellPhoneNo 
        END 
 END      
 
 
 Create PROCEDURE [dbo].[spDownloadGame2]
        -- Add the parameters for the stored procedure here
        @CellPhoneNo        nvarchar(50)='' 
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        SET NOCOUNT ON;

    -- Insert statements for procedure here
    IF EXISTS (SELECT * FROM dbo.Players WHERE CellPhoneNo = @CellPhoneNo)
    AND (SELECT [Game2] FROM [dbo].[DownloadHistory] WHERE CellPhoneNo = @CellPhoneNo) = 0
        BEGIN
			UPDATE		[dbo].[Players]
				SET		[LotteryCount] = [LotteryCount] + 1
				WHERE   [CellPhoneNo] = @CellPhoneNo   
					   
			UPDATE		[dbo].[DownloadHistory]
				SET     [Game2] = 1,
						[Game2Time] = GETDATE()
				WHERE   [CellPhoneNo] = @CellPhoneNo 
        END 
        
END


create PROCEDURE [dbo].[spLotteryProcess]
-- Add the parameters for the stored procedure here
@CellPhoneNo        nvarchar(50)='',
@AwardID                int
AS
BEGIN

	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

	DECLARE @CardCode nvarchar(255)

	-- Check if the phoneNo has the LotteryCount
	IF EXISTS (SELECT * FROM [dbo].[Players] WHERE CellPhoneNo = @CellPhoneNo) 
	AND (SELECT [LotteryCount] FROM [dbo].[Players] WHERE CellPhoneNo = @CellPhoneNo) > 0
	BEGIN
		--Check if the CardCode is exist
		IF (SELECT [SurplusCount] FROM [dbo].[Awards] WHERE [AwardID] = @AwardID) > 0
		BEGIN
			--Get CardCode                  	
			SELECT TOP 1 @CardCode = [CardCode] FROM [dbo].[CardCodes] WHERE [AwardID] = @AwardID AND [Used] = 0 ORDER BY [ID] ASC
	        
			IF (@CardCode = '' or @CardCode is null) and @AwardID <> 11 
			BEGIN
				SET @AwardID = 12
			END
			ELSE
			BEGIN
				--Insert LotteryHistory
				INSERT INTO [dbo].[LotteryHistory]
				   ([CellPhoneNo]
				   ,[LotteryTime]
				   ,[AwardID]
				   ,[CardCode])
				VALUES
				   (@CellPhoneNo
				   ,GETDATE()
				   ,@AwardID
				   ,@CardCode)
	            
				--Update the CardCode to used
				UPDATE	[dbo].[CardCodes]
				SET     [Used] = 1
				WHERE   [AwardID] = @AwardID 
				AND     [CardCode] = @CardCode
	            
				--Update SurplusCount in Awards
				UPDATE  [dbo].[Awards]
				SET     [SurplusCount] = [SurplusCount] - 1
				WHERE   [AwardID] = @AwardID
			END  
		END
		ELSE
		BEGIN
			SET @AwardID = 12
		END

		--Reduce LotteryCount
		UPDATE	[dbo].[Players]
		SET		[LotteryCount] = [LotteryCount] - 1, [SurplusCount] = [SurplusCount] - 1
		WHERE   [CellPhoneNo] = @CellPhoneNo           
	END 

	select @AwardID,@CardCode

END


create PROCEDURE [dbo].[spNewPlayer]
        -- Add the parameters for the stored procedure here
        @CellPhoneNo        nvarchar(50)='' 
AS
BEGIN
        -- SET NOCOUNT ON added to prevent extra result sets from
        SET NOCOUNT ON;

    -- Insert statements for procedure here
    IF NOT EXISTS (SELECT * FROM dbo.Players WHERE CellPhoneNo = @CellPhoneNo)
        BEGIN
           INSERT INTO [dbo].[Players]
					   ([CellPhoneNo]
					   ,[LotteryCount]
					   ,[SurplusCount]
					   ,[LastLoginTime])
				VALUES
					   (@CellPhoneNo
					   ,0
					   ,2
					   ,GETDATE())
					   
			INSERT INTO [dbo].[DownloadHistory]
					   ([CellPhoneNo]
					   ,[Game1]
					   ,[Game1Time]
					   ,[Game2]
					   ,[Game2Time])
				VALUES
					   (@CellPhoneNo
					   ,0
					   ,null
					   ,0
					   ,null)
        END 
        
END