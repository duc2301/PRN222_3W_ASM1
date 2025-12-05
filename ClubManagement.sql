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

-- ==========================================
-- INSERT SAMPLE DATA
-- ==========================================

-- ==========================================
-- 1. INSERT USERS (Dữ liệu người dùng)
-- ==========================================
-- Lưu ý: Password ở đây để dạng text cho dễ test. Thực tế nên hash (MD5/BCrypt).
INSERT INTO Users (username, password, role, full_name, email, phone, department) VALUES 
-- 1. Admin hệ thống
('admin', '123456', 'Admin', N'Nguyễn Quản Trị', 'admin@school.edu.vn', '0901111111', N'Phòng Công Tác Sinh Viên'),

-- 2. Chủ nhiệm các CLB (ClubManager)
('manager_it', '123456', 'ClubManager', N'Lê Code Dạo', 'lecode@school.edu.vn', '0902222222', N'Công nghệ thông tin'),
('manager_music', '123456', 'ClubManager', N'Trần Ca Hát', 'trancahat@school.edu.vn', '0903333333', N'Nghệ thuật'),
('manager_english', '123456', 'ClubManager', N'Phạm Speaking', 'phamspeaking@school.edu.vn', '0904444444', N'Ngôn ngữ Anh'),

-- 3. Sinh viên thường (Student)
('student_a', '123456', 'Student', N'Hoàng Học Giỏi', 'studentA@school.edu.vn', '0905555555', N'Kinh tế'),
('student_b', '123456', 'Student', N'Võ Ham Chơi', 'studentB@school.edu.vn', '0906666666', N'Cơ khí'),
('student_c', '123456', 'Student', N'Đinh Nhiệt Tình', 'studentC@school.edu.vn', '0907777777', N'Công nghệ thông tin'),
('student_d', '123456', 'Student', N'Lý Thụ Động', 'studentD@school.edu.vn', '0908888888', N'Ngoại ngữ');
GO

-- ==========================================
-- 2. INSERT CLUBS (Dữ liệu CLB)
-- ==========================================
-- Giả định ID user theo thứ tự insert trên: 2=manager_it, 3=manager_music, 4=manager_english
INSERT INTO Clubs (club_name, description, leader_id, created_at) VALUES 
(N'CLB Lập Trình (IT Club)', N'Nơi chia sẻ đam mê code, thuật toán và công nghệ.', 2, '2023-09-01'),
(N'CLB Âm Nhạc (Music Club)', N'Giao lưu acoustic, hát, chơi nhạc cụ.', 3, '2023-09-05'),
(N'CLB Tiếng Anh (English Club)', N'Môi trường luyện nói tiếng Anh hàng tuần.', 4, '2023-09-10');
GO

-- ==========================================
-- 3. INSERT MEMBERSHIPS (Thành viên trong CLB)
-- ==========================================
-- Giả định Club ID: 1=IT, 2=Music, 3=English

-- CLB IT (ID=1)
INSERT INTO Memberships (user_id, club_id, role, status, joined_at) VALUES 
(2, 1, 'Leader', 'Active', '2023-09-01'), -- Chủ nhiệm IT
(5, 1, 'Treasurer', 'Active', '2023-09-15'), -- Student A làm thủ quỹ
(7, 1, 'Member', 'Active', '2023-09-20'); -- Student C là thành viên

-- CLB Music (ID=2)
INSERT INTO Memberships (user_id, club_id, role, status, joined_at) VALUES 
(3, 2, 'Leader', 'Active', '2023-09-05'), -- Chủ nhiệm Music
(6, 2, 'Member', 'Inactive', '2023-10-01'), -- Student B (Đã không hoạt động)
(7, 2, 'Member', 'Active', '2023-10-05'); -- Student C tham gia cả Music

-- CLB English (ID=3)
INSERT INTO Memberships (user_id, club_id, role, status, joined_at) VALUES 
(4, 3, 'Leader', 'Active', '2023-09-10'),
(5, 3, 'Member', 'Banned', '2023-11-01'); -- Student A bị ban khỏi CLB Anh văn
GO

-- ==========================================
-- 4. INSERT JOIN REQUESTS (Đơn xin gia nhập)
-- ==========================================
INSERT INTO JoinRequests (user_id, club_id, request_date, status, note) VALUES 
(6, 1, GETDATE(), 'Pending', N'Em muốn học code Python ạ.'), -- Student B xin vào IT (Đang chờ duyệt)
(8, 2, DATEADD(day, -2, GETDATE()), 'Rejected', N'Em hát không hay nhưng hay hát.'), -- Student D xin vào Music (Bị từ chối)
(8, 3, DATEADD(day, -5, GETDATE()), 'Approved', N'I want to improve my skills.'); -- Student D xin vào English (Đã duyệt -> Cần insert vào Memberships nếu logic app chạy, nhưng ở đây chỉ log request)
GO

-- ==========================================
-- 5. INSERT FEES (Các khoản phí)
-- ==========================================
INSERT INTO Fees (club_id, title, amount, due_date, description) VALUES 
(1, N'Quỹ CLB Quý 4/2024', 50000, '2024-12-31', N'Tiền duy trì server và nước uống.'),
(2, N'Vé tham gia Acoustic Night', 100000, '2024-12-25', N'Phí thuê địa điểm và loa đài.');
GO

-- ==========================================
-- 6. INSERT PAYMENTS (Thanh toán)
-- ==========================================
-- Giả định Fee ID: 1=Quỹ IT, 2=Vé Music
INSERT INTO Payments (fee_id, user_id, amount, status, payment_date) VALUES 
(1, 5, 50000, 'Paid', '2024-12-01'), -- Student A đã đóng quỹ IT
(1, 7, 50000, 'Pending', NULL),     -- Student C chưa đóng quỹ IT
(2, 7, 100000, 'Paid', '2024-12-02'); -- Student C đã mua vé Music
GO

-- ==========================================
-- 7. INSERT ACTIVITIES (Hoạt động/Sự kiện)
-- ==========================================
INSERT INTO Activities (club_id, activity_name, description, start_date, end_date, location) VALUES 
(1, N'Workshop: AI cơ bản', N'Giới thiệu về ChatGPT và ứng dụng.', '2024-11-20 08:00:00', '2024-11-20 11:30:00', N'Phòng A101'), -- Sự kiện quá khứ
(1, N'Hackathon Cấp Trường', N'Cuộc thi code 24h.', '2024-12-20 07:00:00', '2024-12-21 07:00:00', N'Hội trường lớn'), -- Sự kiện tương lai
(2, N'Showcase Mùa Đông', N'Biểu diễn văn nghệ cuối năm.', '2024-12-24 19:00:00', '2024-12-24 22:00:00', N'Sân khấu ngoài trời');
GO

-- ==========================================
-- 8. INSERT PARTICIPANTS (Điểm danh)
-- ==========================================
-- Activity ID: 1=Workshop AI, 2=Hackathon, 3=Showcase
INSERT INTO ActivityParticipants (activity_id, user_id, check_in_time, status) VALUES 
-- Workshop AI (Đã diễn ra)
(1, 5, '2024-11-20 08:05:00', 'Attended'), -- Student A đi học
(1, 7, NULL, 'Absent'),                   -- Student C vắng mặt

-- Hackathon (Sắp diễn ra - Chỉ mới đăng ký)
(2, 5, NULL, 'Registered'),
(2, 2, NULL, 'Registered'); -- Manager IT cũng tham gia
GO