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
ALTER TABLE [dbo].[Products]
ADD [Thumb] NVARCHAR(500) NULL; -- ảnh thu nhỏ
Go

-- Bảng Đơn Hàng (Orders)
CREATE TABLE [dbo].[Orders](
	[OrderID] INT PRIMARY KEY IDENTITY(1,1) NOT NULL, -- Mã đơn hàng
    [BuyerID] INT, -- Mã người mua                                                            
    [OrderDate] DATETIME , -- Ngày đặt hàng
    [TransactStatusID] INT NOT NULL, -- Mã trạng thái đơn hàng
    [TotalMoney] DECIMAL(18,2) NOT NULL, -- Tổng tiền     
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

INSERT INTO [dbo].[Users] (Username, Password, Email, PhoneNumber, RoleID, Active)
VALUES 
('admin', '25f9e794323b453885f5181f1b624d0b', 'admin@gmail.com', '0912345678', 3, 1); --mk đã mã hóa , điền 123456789
Go
INSERT INTO [dbo].[Categories] (CatName, [Description]) 
VALUES 
('Thoi Trang', 'San pham thoi trang nhu quan ao, giay dep va phu kien,..'), 
('The Thao', 'Dung cu va trang phuc the thao nhu giay chay bo, vot tennis,...'), 
('Cong Nghe', 'Thiet bi dien tu nhu dien thoai, may tinh, tai nghe,...'), 
('Suc Khoe', 'San pham ho tro suc khoe nhu thuc pham chuc nang, dung cu y te,...');

Go
DELETE FROM Products; -- xóa bản ghi trong bảng

DELETE FROM [Categories];
Go
SELECT MAX(ProductID) FROM Products; -- kiểm tra id hiện tại của bảng
Go
DBCC CHECKIDENT ('Products', RESEED, 0); -- resetID trong bảng(ko có bản ghi nào)
DBCC CHECKIDENT ('Categories', RESEED, 0);

DELETE FROM [Categories];

Go
INSERT INTO [dbo].[Products] (ProductName, [Description], CatID, Price, Quantity, SellerID, DatePosted, ImageURL, ProductStatus) 
VALUES 
-- Thời Trang
('Giay Nike Air Force 1', 'Giay the thao Nike, size 42, mau trang.', 1, 20, 3, 16, GETDATE(), 'https://example.com/nike-af1.jpg', 'Con hang'),
('Ao Khoac Jean Unisex', 'Ao khoac jean thoi trang, size M.', 1, 17, 5, 16, GETDATE(), 'https://example.com/jean-jacket.jpg', 'Con hang'),
('Giay Converse Chuck Taylor', 'Giay Converse dang thoi trang, size 39, mau den.', 1, 18, 4, 16, GETDATE(), 'https://example.com/converse.jpg', 'Con hang'),
('Quan Jeans Nam', 'Quan jeans nam, size L, mau xanh.', 1, 14, 8, 16, GETDATE(), 'https://example.com/jeans.jpg', 'Con hang'),


-- The Thao
('Vot Tennis Wilson', 'Vot tennis Wilson chinh hang.', 2, 14, 2, 16, GETDATE(), 'https://example.com/tennis-racket.jpg', 'Con hang'),
('Bong Da Adidas', 'Bong da Adidas, size 5.', 2, 15, 10, 16, GETDATE(), 'https://example.com/adidas-ball.jpg', 'Con hang'),
('Giay Chay Bo Nike', 'Giay chay bo Nike, size 40, mau xanh la.', 2, 16, 5, 16, GETDATE(), 'https://example.com/nike-running.jpg', 'Con hang'),
('Kinh Da Bơi', 'Kinh da boi chong nuoc, mau xanh.', 2, 11, 10, 16, GETDATE(), 'https://example.com/swim-goggles.jpg', 'Con hang'),


-- Cong Nghe
('iPhone 14 Pro Max', 'iPhone 14 Pro Max moi 99%, bao hanh 12 thang.', 3, 1000, 1, 16, GETDATE(), 'https://example.com/iphone14.jpg', 'Con hang'),
('MacBook Pro 2021', 'MacBook Pro 16 inch, M1 Max, RAM 32GB.', 3, 1500, 1, 16, GETDATE(), 'https://example.com/macbook-pro.jpg', 'Con hang'),
('Tai Nghe Bluetooth Sony', 'Tai nghe Bluetooth Sony chinh hang, mau den.', 3, 2000, 15, 16, GETDATE(), 'https://example.com/sony-headphones.jpg', 'Con hang'),
('Tablet Samsung Galaxy Tab', 'Tablet Samsung Galaxy Tab, RAM 4GB, mau trang.', 3, 1200, 3, 16, GETDATE(), 'https://example.com/samsung-tablet.jpg', 'Con hang'),


-- Suc Khoe
('Vien Uong Vitamin C 500mg', 'Tang cuong de khang va chong oxy hoa.', 4, 4, 50, 16, GETDATE(), 'https://example.com/vitamin-c.jpg', 'Con hang'),
('May Do Huyet Ap Omron', 'May do huyet ap tu dong, bao hanh 24 thang.', 4, 200, 10, 16, GETDATE(), 'https://example.com/blood-pressure.jpg', 'Con hang'),
('Khau Trang Y Te 4 Lop', 'Hop 50 cai, khang khuan.', 4, 4, 100, 16, GETDATE(), 'https://example.com/medical-mask.jpg', 'Con hang'),
('Kem Chong Nang', 'Kem chong nang cho da, bao ve da, mau trang.', 4, 6, 25, 16, GETDATE(), 'https://example.com/sunscreen.jpg', 'Con hang'),
('Bong Banh Ngoai Troi', 'Bong banh ngoai troi, chong nuoc, mau xanh.', 4, 7, 60, 16, GETDATE(), 'https://example.com/outdoor-ball.jpg', 'Con hang');
GO

Go
INSERT INTO [dbo].[Products] (ProductName, [Description], CatID, Price, Quantity, SellerID, DatePosted, ImageURL, ProductStatus) 
VALUES 
-- Thời Trang
('Giày bánh bao Fukau 5', 'Giay thể thao Fukau size 41, màu trắng.', 1, 4, 3, 15, GETDATE(), 'https://example.com/nike-af1.jpg', 'Con hang');
Go
Select*From Products;
Go
UPDATE [dbo].[Products]
SET [Thumb] = 'default2.png'
WHERE [Thumb] = 'default.png';
Go
UPDATE [dbo].[Products]
SET [Thumb] = 'default.png'
WHERE [Thumb] is null;
Go

ALTER TABLE [dbo].[Products] 
ALTER COLUMN [ProductName] NVARCHAR(255) COLLATE Vietnamese_CI_AS;

ALTER TABLE [dbo].[Products] 
ALTER COLUMN [Description] NVARCHAR(MAX) COLLATE Vietnamese_CI_AS;

ALTER TABLE [dbo].[Products] 
ALTER COLUMN [ProductStatus] NVARCHAR(50) COLLATE Vietnamese_CI_AS;


ALTER DATABASE EcC2C COLLATE Vietnamese_CI_AS;

SELECT DATABASEPROPERTYEX('EcC2C', 'Collation') AS DatabaseCollation;

SELECT * FROM Users WHERE Email = 'tph5@gmail.com';
SELECT * FROM Products WHERE SellerID = 16 AND CatID = 1;