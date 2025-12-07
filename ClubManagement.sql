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
('student_d', '123456', 'Student', N'Lý Thụ Động', 'studentD@school.edu.vn', '0908888888', N'Ngoại ngữ'),
('student_e', '123456', 'Student', N'Nguyễn Văn An', 'student_e@school.edu.vn', '0909999999', N'Công nghệ thông tin'),
('student_f', '123456', 'Student', N'Trần Thị Bình', 'student_f@school.edu.vn', '0910101010', N'Kinh tế'),
('student_g', '123456', 'Student', N'Lê Văn Cường', 'student_g@school.edu.vn', '0911111111', N'Ngoại ngữ'),
('student_h', '123456', 'Student', N'Phạm Thị Dung', 'student_h@school.edu.vn', '0912121212', N'Công nghệ thông tin'),
('student_i', '123456', 'Student', N'Đặng Văn Em', 'student_i@school.edu.vn', '0913131313', N'Cơ khí'),
('student_j', '123456', 'Student', N'Vũ Thị Phương', 'student_j@school.edu.vn', '0914141414', N'Nghệ thuật'),
('student_k', '123456', 'Student', N'Bùi Văn Giang', 'student_k@school.edu.vn', '0915151515', N'Kinh tế'),
('student_l', '123456', 'Student', N'Hồ Thị Hoa', 'student_l@school.edu.vn', '0916161616', N'Công nghệ thông tin'),
('student_m', '123456', 'Student', N'Đinh Văn Khánh', 'student_m@school.edu.vn', '0917171717', N'Ngoại ngữ'),
('student_n', '123456', 'Student', N'Võ Thị Lan', 'student_n@school.edu.vn', '0918181818', N'Nghệ thuật'),
('student_o', '123456', 'Student', N'Ngô Văn Minh', 'student_o@school.edu.vn', '0919191919', N'Cơ khí'),
('student_p', '123456', 'Student', N'Mai Thị Nga', 'student_p@school.edu.vn', '0920202020', N'Kinh tế');
GO

-- ==========================================
-- 2. INSERT CLUBS (Dữ liệu CLB)
-- ==========================================
-- Giả định ID user theo thứ tự insert trên: 2=manager_it, 3=manager_music, 4=manager_english
INSERT INTO Clubs (club_name, description, leader_id, created_at) VALUES 
(N'CLB Lập Trình (IT Club)', N'Nơi chia sẻ đam mê code, thuật toán và công nghệ.', 2, '2023-09-01'),
(N'CLB Âm Nhạc (Music Club)', N'Giao lưu acoustic, hát, chơi nhạc cụ.', 3, '2023-09-05'),
(N'CLB Tiếng Anh (English Club)', N'Môi trường luyện nói tiếng Anh hàng tuần.', 4, '2023-09-10'),
(N'CLB Thể Thao (Sports Club)', N'Hoạt động thể dục thể thao và giải trí.', 2, '2024-01-15'),
(N'CLB Nhiếp Ảnh (Photography Club)', N'Chia sẻ kỹ năng chụp ảnh và chỉnh sửa.', 3, '2024-03-20');
GO

-- ==========================================
-- 3. INSERT MEMBERSHIPS (Thành viên trong CLB)
-- ==========================================
-- Giả định Club ID: 1=IT, 2=Music, 3=English, 4=Sports, 5=Photography

-- CLB IT (ID=1) - 8 members
INSERT INTO Memberships (user_id, club_id, role, status, joined_at) VALUES 
(2, 1, 'Leader', 'Active', '2023-09-01'),
(5, 1, 'Treasurer', 'Active', '2023-09-15'),
(7, 1, 'Member', 'Active', '2023-09-20'),
(9, 1, 'Member', 'Active', '2023-10-01'),
(12, 1, 'Member', 'Active', '2024-02-10'),
(16, 1, 'Member', 'Active', '2024-05-15'),
(18, 1, 'Member', 'Active', '2024-06-20'),
(20, 1, 'Member', 'Inactive', '2024-07-01');

-- CLB Music (ID=2) - 6 members
INSERT INTO Memberships (user_id, club_id, role, status, joined_at) VALUES 
(3, 2, 'Leader', 'Active', '2023-09-05'),
(6, 2, 'Member', 'Inactive', '2023-10-01'),
(7, 2, 'Member', 'Active', '2023-10-05'),
(14, 2, 'Member', 'Active', '2024-03-12'),
(17, 2, 'Member', 'Active', '2024-04-18'),
(19, 2, 'Member', 'Active', '2024-08-22');

-- CLB English (ID=3) - 7 members
INSERT INTO Memberships (user_id, club_id, role, status, joined_at) VALUES 
(4, 3, 'Leader', 'Active', '2023-09-10'),
(5, 3, 'Member', 'Banned', '2023-11-01'),
(8, 3, 'Member', 'Active', '2024-01-05'),
(11, 3, 'Member', 'Active', '2024-02-20'),
(13, 3, 'Member', 'Active', '2024-03-15'),
(15, 3, 'Member', 'Active', '2024-06-10'),
(19, 3, 'Member', 'Active', '2024-09-05');

-- CLB Sports (ID=4) - 5 members
INSERT INTO Memberships (user_id, club_id, role, status, joined_at) VALUES 
(2, 4, 'Leader', 'Active', '2024-01-15'),
(6, 4, 'Member', 'Active', '2024-02-01'),
(10, 4, 'Member', 'Active', '2024-03-10'),
(13, 4, 'Member', 'Active', '2024-05-20'),
(17, 4, 'Member', 'Inactive', '2024-07-15');

-- CLB Photography (ID=5) - 4 members
INSERT INTO Memberships (user_id, club_id, role, status, joined_at) VALUES 
(3, 5, 'Leader', 'Active', '2024-03-20'),
(11, 5, 'Member', 'Active', '2024-04-05'),
(14, 5, 'Member', 'Active', '2024-06-15'),
(18, 5, 'Member', 'Active', '2024-08-10');

-- Additional recent memberships (for trends chart - last 6 months)
INSERT INTO Memberships (user_id, club_id, role, status, joined_at) VALUES 
-- July 2025
(10, 1, 'Member', 'Active', '2025-07-05'),
(15, 2, 'Member', 'Active', '2025-07-12'),
(19, 5, 'Member', 'Active', '2025-07-20'),

-- August 2025
(8, 4, 'Member', 'Active', '2025-08-03'),
(17, 1, 'Member', 'Active', '2025-08-15'),

-- September 2025  
(13, 1, 'Member', 'Active', '2025-09-08'),
(10, 3, 'Member', 'Active', '2025-09-18'),
(6, 5, 'Member', 'Active', '2025-09-25'),

-- October 2025
(16, 2, 'Member', 'Active', '2025-10-05'),
(11, 4, 'Member', 'Active', '2025-10-12'),
(15, 5, 'Member', 'Active', '2025-10-22'),

-- November 2025
(8, 1, 'Member', 'Active', '2025-11-02'),
(19, 4, 'Member', 'Active', '2025-11-10'),
(12, 2, 'Member', 'Active', '2025-11-18'),
(17, 3, 'Member', 'Active', '2025-11-25'),

-- December 2025 (current month)
(10, 5, 'Member', 'Active', '2025-12-01'),
(16, 4, 'Member', 'Active', '2025-12-03');
GO

-- ==========================================
-- 4. INSERT JOIN REQUESTS (Đơn xin gia nhập)
-- ==========================================
INSERT INTO JoinRequests (user_id, club_id, request_date, status, note) VALUES 
-- Pending requests
(6, 1, GETDATE(), 'Pending', N'Em muốn học code Python ạ.'),
(10, 2, DATEADD(day, -1, GETDATE()), 'Pending', N'Em muốn tham gia biểu diễn.'),
(12, 3, DATEADD(day, -3, GETDATE()), 'Pending', N'Muốn cải thiện kỹ năng giao tiếp.'),
(15, 1, DATEADD(day, -2, GETDATE()), 'Pending', N'Quan tâm đến AI và Machine Learning.'),
(19, 4, GETDATE(), 'Pending', N'Thích chơi bóng đá.'),

-- Approved requests
(8, 3, DATEADD(day, -5, GETDATE()), 'Approved', N'I want to improve my skills.'),
(11, 3, DATEADD(day, -120, GETDATE()), 'Approved', N'Looking forward to joining.'),
(13, 4, DATEADD(day, -80, GETDATE()), 'Approved', N'Em thích thể thao.'),
(14, 2, DATEADD(day, -150, GETDATE()), 'Approved', N'Muốn học đàn guitar.'),

-- Rejected requests
(8, 2, DATEADD(day, -2, GETDATE()), 'Rejected', N'Em hát không hay nhưng hay hát.'),
(16, 3, DATEADD(day, -7, GETDATE()), 'Rejected', N'Chưa đủ điều kiện tham gia.'),
(20, 5, DATEADD(day, -10, GETDATE()), 'Rejected', N'Chưa có kinh nghiệm chụp ảnh.');
GO

-- ==========================================
-- 5. INSERT FEES (Các khoản phí)
-- ==========================================
INSERT INTO Fees (club_id, title, amount, due_date, description) VALUES 
-- CLB IT
(1, N'Quỹ CLB Quý 4/2024', 50000, '2024-12-31', N'Tiền duy trì server và nước uống.'),
(1, N'Phí Workshop AI', 30000, '2024-11-15', N'Chi phí tài liệu và giảng viên.'),
(1, N'Phí Hackathon', 100000, '2024-12-18', N'Lệ phí tham gia cuộc thi.'),

-- CLB Music
(2, N'Vé tham gia Acoustic Night', 100000, '2024-12-25', N'Phí thuê địa điểm và loa đài.'),
(2, N'Quỹ CLB Tháng 12', 40000, '2024-12-20', N'Phí duy trì hoạt động.'),

-- CLB English
(3, N'Phí thi TOEIC Mock Test', 80000, '2024-12-15', N'Chi phí đề thi và chấm bài.'),
(3, N'Quỹ hoạt động Quý 4', 35000, '2024-12-30', N'Tiền nước và tài liệu.'),

-- CLB Sports
(4, N'Phí thuê sân bóng', 60000, '2024-12-10', N'Thuê sân thi đấu hàng tháng.'),
(4, N'Quỹ mua đồ thể thao', 150000, '2025-01-05', N'Mua bóng và áo đấu.'),

-- CLB Photography
(5, N'Phí chuyến chụp ảnh', 120000, '2024-12-28', N'Chi phí đi chuyến Đà Lạt.');
GO

-- ==========================================
-- 6. INSERT PAYMENTS (Thanh toán)
-- ==========================================
INSERT INTO Payments (fee_id, user_id, amount, status, payment_date) VALUES 
-- Fee 1: Quỹ CLB IT Quý 4
(1, 5, 50000, 'Paid', '2024-12-01'),
(1, 7, 50000, 'Pending', NULL),
(1, 9, 50000, 'Paid', '2024-12-03'),
(1, 12, 50000, 'Paid', '2024-11-28'),

-- Fee 2: Phí Workshop AI
(2, 5, 30000, 'Paid', '2024-11-10'),
(2, 7, 30000, 'Paid', '2024-11-12'),
(2, 9, 30000, 'Pending', NULL),

-- Fee 3: Phí Hackathon
(3, 5, 100000, 'Pending', NULL),
(3, 12, 100000, 'Pending', NULL),

-- Fee 4: Vé Acoustic Night
(4, 7, 100000, 'Paid', '2024-12-02'),
(4, 14, 100000, 'Paid', '2024-12-05'),
(4, 17, 100000, 'Pending', NULL),

-- Fee 5: Quỹ Music Tháng 12
(5, 7, 40000, 'Paid', '2024-12-01'),
(5, 14, 40000, 'Pending', NULL),

-- Fee 6: TOEIC Mock Test
(6, 8, 80000, 'Paid', '2024-12-08'),
(6, 11, 80000, 'Paid', '2024-12-09'),
(6, 13, 80000, 'Pending', NULL),

-- Fee 7: Quỹ English Quý 4
(7, 8, 35000, 'Pending', NULL),
(7, 11, 35000, 'Paid', '2024-12-10'),

-- Fee 8: Phí thuê sân
(8, 6, 60000, 'Paid', '2024-12-01'),
(8, 10, 60000, 'Pending', NULL),

-- Fee 9: Quỹ mua đồ thể thao
(9, 6, 150000, 'Pending', NULL),

-- Fee 10: Chuyến chụp ảnh
(10, 11, 120000, 'Paid', '2024-12-15'),
(10, 14, 120000, 'Pending', NULL);
GO

-- ==========================================
-- 7. INSERT ACTIVITIES (Hoạt động/Sự kiện)
-- ==========================================
INSERT INTO Activities (club_id, activity_name, description, start_date, end_date, location) VALUES 
-- CLB IT - Past events
(1, N'Workshop: AI cơ bản', N'Giới thiệu về ChatGPT và ứng dụng.', '2024-11-20 08:00:00', '2024-11-20 11:30:00', N'Phòng A101'),
(1, N'Code Competition 2024', N'Cuộc thi lập trình nội bộ.', '2024-10-15 09:00:00', '2024-10-15 17:00:00', N'Phòng Máy B201'),

-- CLB IT - Upcoming events
(1, N'Hackathon Cấp Trường', N'Cuộc thi code 24h.', '2024-12-20 07:00:00', '2024-12-21 07:00:00', N'Hội trường lớn'),
(1, N'Seminar: Web Development', N'ReactJS và Node.js cho beginners.', '2025-01-10 14:00:00', '2025-01-10 17:00:00', N'Phòng A305'),
(1, N'Tech Talk: Cloud Computing', N'AWS và Azure overview.', '2025-01-25 15:00:00', '2025-01-25 18:00:00', N'Hội trường nhỏ'),

-- CLB Music - Past events
(2, N'Practice Session Tuần 1', N'Luyện tập acoustic.', '2024-11-05 18:00:00', '2024-11-05 20:00:00', N'Phòng âm nhạc'),
(2, N'Practice Session Tuần 2', N'Luyện hát hòa âm.', '2024-11-12 18:00:00', '2024-11-12 20:00:00', N'Phòng âm nhạc'),

-- CLB Music - Upcoming events
(2, N'Showcase Mùa Đông', N'Biểu diễn văn nghệ cuối năm.', '2024-12-24 19:00:00', '2024-12-24 22:00:00', N'Sân khấu ngoài trời'),
(2, N'Acoustic Night Vol.2', N'Đêm nhạc acoustic thứ 2.', '2025-01-15 19:30:00', '2025-01-15 22:00:00', N'Cafe Sân Vườn'),

-- CLB English - Past events
(3, N'English Conversation Club', N'Giao lưu tiếng Anh hàng tuần.', '2024-11-18 17:00:00', '2024-11-18 19:00:00', N'Phòng E102'),

-- CLB English - Upcoming events
(3, N'TOEIC Mock Test', N'Thi thử TOEIC.', '2024-12-14 08:00:00', '2024-12-14 10:00:00', N'Phòng thi E201'),
(3, N'Debate Competition', N'Cuộc thi tranh biện tiếng Anh.', '2024-12-28 14:00:00', '2024-12-28 17:00:00', N'Hội trường nhỏ'),
(3, N'Movie Night: English Films', N'Xem phim và thảo luận.', '2025-01-08 18:00:00', '2025-01-08 21:00:00', N'Phòng chiếu phim'),

-- CLB Sports - Past events
(4, N'Giao hữu bóng đá', N'Giao hữu với CLB khác.', '2024-11-10 16:00:00', '2024-11-10 18:00:00', N'Sân bóng trường'),

-- CLB Sports - Upcoming events
(4, N'Giải bóng đá nội bộ', N'Thi đấu giữa các lớp.', '2024-12-15 15:00:00', '2024-12-15 18:00:00', N'Sân bóng chính'),
(4, N'Chạy marathon từ thiện', N'Chạy gây quỹ cho trẻ em.', '2025-01-20 06:00:00', '2025-01-20 09:00:00', N'Công viên thành phố'),

-- CLB Photography - Upcoming events
(5, N'Chuyến chụp ảnh Đà Lạt', N'Tour chụp ảnh 3 ngày 2 đêm.', '2024-12-27 06:00:00', '2024-12-29 18:00:00', N'Đà Lạt'),
(5, N'Workshop: Portrait Photography', N'Kỹ thuật chụp chân dung.', '2025-01-12 14:00:00', '2025-01-12 17:00:00', N'Studio A');
GO

-- ==========================================
-- 8. INSERT PARTICIPANTS (Điểm danh)
-- ==========================================
INSERT INTO ActivityParticipants (activity_id, user_id, check_in_time, status) VALUES 
-- Activity 1: Workshop AI (Past)
(1, 5, '2024-11-20 08:05:00', 'Attended'),
(1, 7, NULL, 'Absent'),
(1, 9, '2024-11-20 08:10:00', 'Attended'),
(1, 12, '2024-11-20 08:02:00', 'Attended'),

-- Activity 2: Code Competition (Past)
(2, 5, '2024-10-15 09:15:00', 'Attended'),
(2, 7, '2024-10-15 09:05:00', 'Attended'),
(2, 9, NULL, 'Absent'),
(2, 12, '2024-10-15 09:20:00', 'Attended'),
(2, 16, '2024-10-15 09:10:00', 'Attended'),

-- Activity 3: Hackathon (Upcoming - Registered)
(3, 5, NULL, 'Registered'),
(3, 2, NULL, 'Registered'),
(3, 7, NULL, 'Registered'),
(3, 9, NULL, 'Registered'),
(3, 12, NULL, 'Registered'),
(3, 16, NULL, 'Registered'),

-- Activity 4: Seminar Web Dev (Upcoming)
(4, 5, NULL, 'Registered'),
(4, 7, NULL, 'Registered'),
(4, 16, NULL, 'Registered'),

-- Activity 5: Tech Talk Cloud (Upcoming)
(5, 9, NULL, 'Registered'),
(5, 12, NULL, 'Registered'),

-- Activity 6: Music Practice Session 1 (Past)
(6, 7, '2024-11-05 18:05:00', 'Attended'),
(6, 14, '2024-11-05 18:10:00', 'Attended'),
(6, 17, NULL, 'Absent'),

-- Activity 7: Music Practice Session 2 (Past)
(7, 7, '2024-11-12 18:00:00', 'Attended'),
(7, 14, '2024-11-12 18:08:00', 'Attended'),
(7, 17, '2024-11-12 18:15:00', 'Attended'),
(7, 19, NULL, 'Absent'),

-- Activity 8: Showcase (Upcoming)
(8, 7, NULL, 'Registered'),
(8, 14, NULL, 'Registered'),
(8, 17, NULL, 'Registered'),
(8, 19, NULL, 'Registered'),

-- Activity 9: Acoustic Night Vol.2 (Upcoming)
(9, 7, NULL, 'Registered'),
(9, 14, NULL, 'Registered'),

-- Activity 10: English Conversation (Past)
(10, 8, '2024-11-18 17:05:00', 'Attended'),
(10, 11, '2024-11-18 17:02:00', 'Attended'),
(10, 13, NULL, 'Absent'),
(10, 15, '2024-11-18 17:10:00', 'Attended'),

-- Activity 11: TOEIC Mock Test (Upcoming)
(11, 8, NULL, 'Registered'),
(11, 11, NULL, 'Registered'),
(11, 13, NULL, 'Registered'),
(11, 15, NULL, 'Registered'),
(11, 19, NULL, 'Registered'),

-- Activity 12: Debate Competition (Upcoming)
(12, 8, NULL, 'Registered'),
(12, 11, NULL, 'Registered'),
(12, 13, NULL, 'Registered'),

-- Activity 13: Movie Night (Upcoming)
(13, 15, NULL, 'Registered'),
(13, 19, NULL, 'Registered'),

-- Activity 14: Giao hữu bóng đá (Past)
(14, 6, '2024-11-10 16:05:00', 'Attended'),
(14, 10, '2024-11-10 16:00:00', 'Attended'),
(14, 13, NULL, 'Absent'),

-- Activity 15: Giải bóng đá nội bộ (Upcoming)
(15, 6, NULL, 'Registered'),
(15, 10, NULL, 'Registered'),
(15, 13, NULL, 'Registered'),

-- Activity 16: Marathon (Upcoming)
(16, 6, NULL, 'Registered'),
(16, 10, NULL, 'Registered'),

-- Activity 17: Chuyến Đà Lạt (Upcoming)
(17, 11, NULL, 'Registered'),
(17, 14, NULL, 'Registered'),
(17, 18, NULL, 'Registered'),

-- Activity 18: Workshop Portrait (Upcoming)
(18, 11, NULL, 'Registered'),
(18, 14, NULL, 'Registered');
GO