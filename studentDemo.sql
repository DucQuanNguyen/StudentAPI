Use studentDemo
Go
CREATE TABLE [dbo].[LopHoc](
	[ID] [int] NOT NULL primary key,
	[ClassName] [nvarchar](max) NOT NULL,
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