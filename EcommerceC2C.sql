CREATE DATABASE EcC2C

Use EcC2C
-- 5 người dung u1 role1, u2 role2 ,u3 role1 ,u4 role 2,u5 role 2
-- Bảng Người Dùng (Users)
CREATE TABLE [dbo].[Users] (
    [UserID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL, -- Mã người dùng
    [Username] VARCHAR(50) NOT NULL UNIQUE, -- Tên đăng nhập
    [Password] VARCHAR(255) NOT NULL, -- Mật khẩu
    [Email] VARCHAR(100) NOT NULL UNIQUE, -- Email
    [PhoneNumber] VARCHAR(30), -- Số điện thoại
    [RoleID] INT NOT NULL, -- Mã vai trò
    [Active] BIT NOT NULL DEFAULT 1, -- Trạng thái hoạt động
    [CreateDate] DATETIME , -- Ngày tạo
);
Go
-- Bảng Vai Trò (Roles)
CREATE TABLE [dbo].[Roles](
	[RoleID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL, -- Mã vai trò
    [RoleName] NVARCHAR(50) NOT NULL, -- Tên vai trò
    [Description] NVARCHAR(255) -- Mô tả vai trò
);

Go
-- Bảng Danh Mục (Categories)
CREATE TABLE [dbo].[Categories](
	[CatID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL, -- Mã danh mục
    [CatName] NVARCHAR(250) NOT NULL, -- Tên danh mục
    [Description] NVARCHAR(MAX) -- Mô tả danh mục
);
Go
-- Bảng Sản Phẩm (Product)
CREATE TABLE [dbo].[Products] (
    [ProductID] INT PRIMARY KEY IDENTITY(1,1), -- Mã sản phẩm
    [ProductName] NVARCHAR(255) NOT NULL, -- Tên sản phẩm
    [Description] NVARCHAR(MAX), -- Mô tả sản phẩm
    [CatID] INT NOT NULL, -- Mã danh mục
    [Price] DECIMAL(18, 2) NOT NULL, -- Giá sản phẩm
    [Quantity] INT NOT NULL, -- Số lượng
    [SellerID] INT, -- Mã người bán
    [DatePosted] DATETIME  , -- Ngày đăng
	[ImageURL] NVARCHAR(500) NULL, -- Đường dẫn ảnh sản phẩm
    [ProductStatus] NVARCHAR(50) -- Trạng thái sản phẩm (còn hàng hay hết hàng)
    
);
Go

-- Bảng Đơn Hàng (Orders)
CREATE TABLE [dbo].[Orders](
	[OrderID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL, -- Mã đơn hàng
    [BuyerID] INT, -- Mã người mua                                                            
    [OrderDate] DATETIME , -- Ngày đặt hàng
    [TransactStatusID] INT NOT NULL, -- Mã trạng thái đơn hàng
    [PaymentDate] DATETIME, -- Ngày thanh toán
    [TotalMoney] DECIMAL(18,2) NOT NULL, -- Tổng tiền
    [PaymentID] INT, -- Mã thanh toán             
    [Note] NVARCHAR(MAX), -- Ghi chú
    [Address] NVARCHAR(MAX), -- Địa chỉ giao hàng
)
Go
-- Bảng Chi Tiết Đơn Hàng (OrderDetails)
CREATE TABLE [dbo].[OrderDetails](
	[OrderDetailID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL, -- Mã chi tiết đơn hàng
    [OrderID] INT NOT NULL, -- Mã đơn hàng
    [ProductID] INT NOT NULL, -- Mã sản phẩm
    [OrderNumber] INT NOT NULL, -- Số thứ tự đơn hàng
    [Quantity] INT NOT NULL, -- Số lượng
    [TotalMoney] DECIMAL(18,2) NOT NULL, -- Tổng tiền
    [CreateDate] DATETIME , -- Ngày tạo
    [Price] DECIMAL(18,2) NOT NULL -- Giá bán
)
-- Bảng Trạng Thái Giao Dịch (TransactStatus)
CREATE TABLE [dbo].[TransactStatus](
	[TransactStatusID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL, -- Mã trạng thái giao dịch
    [Status] NVARCHAR(50) NOT NULL, -- Trạng thái
    [Description] NVARCHAR(MAX) -- Mô tả trạng thái
)
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Roles] FOREIGN KEY([RoleID])
REFERENCES [dbo].[Roles] ([RoleID])
GO
ALTER TABLE [dbo].[Products] ADD CONSTRAINT [FK_Product_Seller] FOREIGN KEY ([SellerID]) REFERENCES [dbo].[Users]([UserID])
GO
ALTER TABLE [dbo].[Orders] ADD CONSTRAINT [FK_Order_Buyer] FOREIGN KEY ([BuyerID]) REFERENCES [Users]([UserID])
GO
ALTER TABLE [dbo].[OrderDetails] ADD CONSTRAINT [FK_OrderDetail_Order] FOREIGN KEY ([OrderID]) REFERENCES [Orders]([OrderID])
GO
ALTER TABLE [dbo].[OrderDetails] ADD CONSTRAINT [FK_OrderDetail_Product] FOREIGN KEY ([ProductID]) REFERENCES [Products]([ProductID])
Go
ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK_Products_Categories] FOREIGN KEY([CatID])
REFERENCES [dbo].[Categories] ([CatID])
Go
ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK_Orders_TransactStatus] FOREIGN KEY([TransactStatusID])
REFERENCES [dbo].[TransactStatus] ([TransactStatusID])

INSERT INTO [dbo].[Roles] (RoleName, Description)
VALUES 
('Buyer', 'Người mua hàng'),
('Seller', 'Người bán hàng'),
('Admin', 'Quản trị viên');

