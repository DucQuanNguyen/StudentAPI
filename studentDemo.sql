CREATE DATABASE studentDemo
GO
Use studentDemo
Go
CREATE TABLE [dbo].[LopHoc](
	[ID] [int] NOT NULL primary key,
	[ClassName] [nvarchar](30) NOT NULL,
)
GO
insert into [LopHoc] values('1','SE0001')
insert into [LopHoc] values('2','SE0002')
insert into [LopHoc] values('3','JS0001')
insert into [LopHoc] values('4','KS0001')
insert into [LopHoc] values('5','BA0001')
GO
CREATE TABLE [dbo].[SinhVien](
	[StudentID] [nvarchar](10) NOT NULL primary key,
	[StudentName] [nvarchar](30) NOT NULL,
	[BirthDate] datetime not NULL,
	[Gender] [nvarchar](10) NULL,
	[ClassID] [int] NULL references [LopHoc]([ID])
)
GO
insert into [SinhVien] values('SE05001','Dao Thanh Hung',CAST(N'2001-04-04' AS Date),'Female','1')
insert into [SinhVien] values('SE05002','Tran Hung Dung',CAST(N'2003-03-04' AS Date),'Female','2')
insert into [SinhVien] values('SE05003','Le Khanh Van',CAST(N'2001-05-04' AS Date),'Male','2')
insert into [SinhVien] values('SE05008','Tran Hung Son',CAST(N'2001-10-02' AS Date),'Female','3')
insert into [SinhVien] values('SE05009','Le Khanh Ha',CAST(N'1999-02-04' AS Date),'Male','3')
insert into [SinhVien] values('SE05031','Ha Hung Dung',CAST(N'2001-03-01' AS Date),'Female','4')
insert into [SinhVien] values('SE05201','Tran Hung Cuong',CAST(N'2001-11-06' AS Date),'Female','5')
insert into [SinhVien] values('SE05102','Le Khanh Van',CAST(N'1999-01-05' AS Date),'Male','5')
insert into [SinhVien] values('SE05603','Nguyen Phu Dat',CAST(N'2003-04-04' AS Date),'Female','5')
Go
CREATE TABLE [dbo].[User](
	[ID] [int] NOT NULL primary key,
	[UserName] [nvarchar](50) NOT NULL,
	[PassWord] [nvarchar](50) NOT NULL,
	[Role] [nvarchar](5) NOT NULL,
)
GO
insert into [User] values(1,'user1','@123','user')
insert into [User] values(2,'admin','@123','admin')
GO
CREATE PROC GETLopHoc
AS
BEGIN
	SELECT [ID]
           ,[ClassName]
		   FROM [dbo].[LopHoc]
END
GO
CREATE PROC GETLopHocbyId(
	@ID int)
AS
BEGIN
	SELECT [ID]
           ,[ClassName]
		   FROM [dbo].[LopHoc] 
		   WHERE ID = @ID
END
GO
CREATE PROC AddLopHoc(
	@ID int,
	@ClassName nvarchar(30))
AS
BEGIN
	INSERT INTO [dbo].[LopHoc]
           ([ID]
           ,[ClassName])
     VALUES
           (@ID,
			@ClassName)
END
GO
CREATE PROC UpdateLopHoc(
	@ID nvarchar(10),
	@ClassName nvarchar(30))
AS
BEGIN
	UPDATE [dbo].[LopHoc]
	SET [ClassName] = @ClassName
	WHERE [ID] = @ID
END
GO
CREATE PROC DeleteLopHoc(
	@ID nvarchar(10))
AS
BEGIN
	DELETE FROM [dbo].[LopHoc]
	WHERE [ID] = @ID
END
Go
CREATE PROC GETSinhVien
AS
BEGIN
	SELECT [StudentID]
           ,[StudentName]
           ,[BirthDate]
           ,[Gender]
           ,[ClassID]
		   FROM [dbo].[SinhVien]
END
GO
CREATE PROC GETSinhVienById(
	@StudentID nvarchar(10)
)
AS
BEGIN
	SELECT [StudentID]
           ,[StudentName]
           ,[BirthDate]
           ,[Gender]
           ,[ClassID]
		   FROM [dbo].[SinhVien]
		   WHERE [StudentID] = @StudentID
END
GO
CREATE PROC AddSinhVien(
	@StudentID nvarchar(10),
	@StudentName nvarchar(30),
	@BirthDate datetime,
	@Gender nvarchar(10),
	@ClassID int)
AS
BEGIN
	INSERT INTO [dbo].[SinhVien]
           ([StudentID]
           ,[StudentName]
           ,[BirthDate]
           ,[Gender]
           ,[ClassID])
     VALUES
           (@StudentID,
			@StudentName,
			@BirthDate,
			@Gender,
			@ClassID)
END
GO
CREATE PROC UpdateSinhVien(
	@StudentID nvarchar(10),
	@StudentName nvarchar(30),
	@BirthDate datetime,
	@Gender nvarchar(10),
	@ClassID int)
AS
BEGIN
	UPDATE [dbo].[SinhVien]
	SET [StudentName] = @StudentName
      ,[BirthDate] = @BirthDate
      ,[Gender] = @Gender
      ,[ClassID] = @ClassID
	WHERE [StudentID] = @StudentID
END
GO
CREATE PROC DeleteSinhVien(
	@StudentID nvarchar(10))
AS
BEGIN
	DELETE FROM [dbo].[SinhVien]
	WHERE [StudentID] = @StudentID
END
Go
exec GETLopHocbyId @ID=2