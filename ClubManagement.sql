USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'ClubManagement')
BEGIN
	ALTER DATABASE ClubManagement SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
	DROP DATABASE ClubManagement;
END
GO

CREATE DATABASE ClubManagement;
GO

USE ClubManagement;
GO

-- ==========================================
-- 1. Users (Gộp Students và Users cũ)
-- ==========================================
-- Bảng này chứa cả thông tin cá nhân và thông tin đăng nhập
CREATE TABLE Users (
    user_id INT IDENTITY(1,1) PRIMARY KEY,
    username NVARCHAR(50) UNIQUE NOT NULL, 
    password NVARCHAR(255) NOT NULL, 
    role VARCHAR(20) NOT NULL DEFAULT 'Student', 
    
    full_name NVARCHAR(100) NOT NULL,
    email NVARCHAR(100) UNIQUE NOT NULL,
    phone NVARCHAR(20),
    department NVARCHAR(100), 
    
    created_at DATETIME DEFAULT GETDATE(),

    CONSTRAINT CK_User_Role CHECK (role IN ('Admin', 'ClubManager', 'Student'))
);
GO

-- ==========================================
-- 2. Clubs (Thông tin CLB)
-- ==========================================
CREATE TABLE Clubs (
    club_id INT IDENTITY(1,1) PRIMARY KEY,
    club_name NVARCHAR(100) UNIQUE NOT NULL,
    description NVARCHAR(MAX),
    created_at DATETIME DEFAULT GETDATE(),
    leader_id INT NULL, 
    
    FOREIGN KEY (leader_id) REFERENCES Users(user_id)
);
GO

-- ==========================================
-- 3. Memberships (Quản lý thành viên CLB)
-- ==========================================
CREATE TABLE Memberships (
    membership_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL,
    club_id INT NOT NULL,
    role VARCHAR(20) NOT NULL DEFAULT 'Member', -- 'Member', 'Leader', 'Treasurer'
    joined_at DATETIME DEFAULT GETDATE(),
    status VARCHAR(20) NOT NULL DEFAULT 'Active', -- 'Active', 'Inactive', 'Banned'

    CONSTRAINT UQ_Member UNIQUE(user_id, club_id),
    CONSTRAINT CK_Membership_Status CHECK (status IN ('Active', 'Inactive', 'Banned')),
    FOREIGN KEY(user_id) REFERENCES Users(user_id),
    FOREIGN KEY(club_id) REFERENCES Clubs(club_id)
);
GO

-- ==========================================
-- 4. Join Requests (Đơn xin gia nhập)
-- ==========================================
CREATE TABLE JoinRequests (
    request_id INT IDENTITY(1,1) PRIMARY KEY,
    user_id INT NOT NULL, 
    club_id INT NOT NULL,
    request_date DATETIME DEFAULT GETDATE(),
    status VARCHAR(20) NOT NULL DEFAULT 'Pending', -- 'Pending', 'Approved', 'Rejected'
    note NVARCHAR(MAX), 

    CONSTRAINT CK_Request_Status CHECK (status IN ('Pending', 'Approved', 'Rejected')),
    FOREIGN KEY(user_id) REFERENCES Users(user_id),
    FOREIGN KEY(club_id) REFERENCES Clubs(club_id)
);
GO

-- ==========================================
-- 5. Fees (Các khoản thu: Phí thường niên, phí sự kiện...)
-- ==========================================
CREATE TABLE Fees (
    fee_id INT IDENTITY(1,1) PRIMARY KEY,
    club_id INT NOT NULL,
    title NVARCHAR(100) NOT NULL, 
    amount DECIMAL(10,2) NOT NULL,
    due_date DATE NOT NULL,
    description NVARCHAR(255),
    created_at DATETIME DEFAULT GETDATE(),

    FOREIGN KEY(club_id) REFERENCES Clubs(club_id)
);
GO

-- ==========================================
-- 6. Payments 
-- ==========================================
CREATE TABLE Payments (
    payment_id INT IDENTITY(1,1) PRIMARY KEY,
    fee_id INT NOT NULL,
    user_id INT NOT NULL, 
    amount DECIMAL(10,2) NOT NULL,
    payment_date DATETIME DEFAULT GETDATE(),
    status VARCHAR(20) NOT NULL DEFAULT 'Paid', -- 'Paid', 'Pending', 'Expired'

    CONSTRAINT UQ_Payment UNIQUE(fee_id, user_id),
    FOREIGN KEY(fee_id) REFERENCES Fees(fee_id),
    FOREIGN KEY(user_id) REFERENCES Users(user_id)
);
GO

-- ==========================================
-- 7. Activities (Sự kiện/Hoạt động)
-- ==========================================
CREATE TABLE Activities (
    activity_id INT IDENTITY(1,1) PRIMARY KEY,
    club_id INT NOT NULL,
    activity_name NVARCHAR(150) NOT NULL,
    description NVARCHAR(MAX),
    start_date DATETIME NOT NULL,
    end_date DATETIME NULL,
    location NVARCHAR(255), 
    FOREIGN KEY(club_id) REFERENCES Clubs(club_id)
);
GO

-- ==========================================
-- 8. Activity Participants (Điểm danh tham gia) 
-- ==========================================
CREATE TABLE ActivityParticipants (
    participant_id INT IDENTITY(1,1) PRIMARY KEY,
    activity_id INT NOT NULL,
    user_id INT NOT NULL, 
    check_in_time DATETIME DEFAULT GETDATE(),
    status NVARCHAR(50) DEFAULT 'Attended', -- 'Registered', 'Attended', 'Absent'

    CONSTRAINT UQ_Activity_User UNIQUE(activity_id, user_id),
    FOREIGN KEY(activity_id) REFERENCES Activities(activity_id),
    FOREIGN KEY(user_id) REFERENCES Users(user_id)
);
GO