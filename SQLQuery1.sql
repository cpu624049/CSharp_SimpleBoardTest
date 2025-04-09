--CREATE TABLE Board_Users (
--    UserId INT IDENTITY(1,1) PRIMARY KEY,
--    UserName NVARCHAR(100) NOT NULL,
--    Email NVARCHAR(200) NOT NULL UNIQUE,
--    Password NVARCHAR(255) NOT NULL, -- 암호화된 문자열 저장
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL
--);

--CREATE TABLE Board_Posts (
--    PostId INT IDENTITY(1,1) PRIMARY KEY,
--    UserId INT NOT NULL,
--    Title NVARCHAR(200) NOT NULL,
--    Content NVARCHAR(MAX) NOT NULL,
--    ViewCount INT NOT NULL DEFAULT 0,
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL,
--    FOREIGN KEY (UserId) REFERENCES Board_Users(UserId)
--);

--CREATE TABLE Board_Comments (
--    CommentId INT IDENTITY(1,1) PRIMARY KEY,
--    PostId INT NOT NULL,
--    UserId INT NOT NULL,
--    ParentCommentId INT NULL,
--    Content NVARCHAR(MAX) NOT NULL,
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL,
--    FOREIGN KEY (PostId) REFERENCES Board_Posts(PostId),
--    FOREIGN KEY (UserId) REFERENCES Board_Users(UserId),
--    FOREIGN KEY (ParentCommentId) REFERENCES Board_Comments(CommentId)
--);

--CREATE TABLE Board_PostLikes (
--    PostLikeId INT IDENTITY(1,1) PRIMARY KEY,
--    PostId INT NOT NULL,
--    UserId INT NOT NULL,
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    CONSTRAINT UQ_PostLike UNIQUE(PostId, UserId), -- 중복 방지
--    FOREIGN KEY (PostId) REFERENCES Board_Posts(PostId),
--    FOREIGN KEY (UserId) REFERENCES Board_Users(UserId)
--);

--CREATE TABLE Board_CommentLikes (
--    CommentLikeId INT IDENTITY(1,1) PRIMARY KEY,
--    CommentId INT NOT NULL,
--    UserId INT NOT NULL,
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    CONSTRAINT UQ_CommentLike UNIQUE(CommentId, UserId), -- 중복 방지
--    FOREIGN KEY (CommentId) REFERENCES Board_Comments(CommentId),
--    FOREIGN KEY (UserId) REFERENCES Board_Users(UserId)
--);

--CREATE TABLE Board_ReplyPosts (
--    ReplyPostId INT IDENTITY(1,1) PRIMARY KEY,
--    ParentPostId INT NOT NULL,           -- 원본 글 ID
--    UserId INT NOT NULL,                 -- 작성자
--    Title NVARCHAR(200) NOT NULL,        -- 제목 (일반 글과 동일)
--    Content NVARCHAR(MAX) NOT NULL,      -- 내용
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL,
--    FOREIGN KEY (ParentPostId) REFERENCES Board_Posts(PostId),
--    FOREIGN KEY (UserId) REFERENCES Users(UserId)
--);


--ALTER TABLE Board_Posts ADD ParentPostId INT NULL;

--ALTER TABLE Board_Posts ADD CONSTRAINT FK_Board_Posts_ParentPost FOREIGN KEY (ParentPostId) REFERENCES Board_Posts(PostId);

--SELECT COLUMN_NAME 
--FROM INFORMATION_SCHEMA.COLUMNS 
--WHERE TABLE_NAME = 'Board_Posts';

-----------------------------------------------

--select @@VERSION;

--Microsoft SQL Server 2012 (SP4-GDR) (KB4583465) - 11.0.7507.2 (X64)   Nov  1 2020 00:48:37
--Copyright (c) Microsoft Corporation  Standard Edition (64-bit) on Windows NT 6.3 <X64> (Build 9600: ) 


-----------------------------------------------

---- 좋아요 테이블부터 삭제 (외래키 종속성 때문)
--IF OBJECT_ID('dbo.Board_CommentLikes', 'U') IS NOT NULL
--	DROP TABLE dbo.Board_CommentLikes;

--IF OBJECT_ID('dbo.Board_PostLikes', 'U') IS NOT NULL
--	DROP TABLE dbo.Board_PostLikes;

---- 댓글 테이블 삭제
--IF OBJECT_ID('dbo.Board_Comments', 'U') IS NOT NULL
--	DROP TABLE dbo.Board_Comments;

---- 답글 테이블 삭제
--IF OBJECT_ID('dbo.Board_ReplyPosts', 'U') IS NOT NULL
--	DROP TABLE dbo.Board_ReplyPosts;

---- 게시글 테이블 삭제
--IF OBJECT_ID('dbo.Board_Posts', 'U') IS NOT NULL
--	DROP TABLE dbo.Board_Posts;

---- 유저 테이블 삭제
--IF OBJECT_ID('dbo.Board_Users', 'U') IS NOT NULL
--	DROP TABLE dbo.Board_Users;

-----------------------------------------------

------ 테이블 재생성

---- 사용자 테이블
--CREATE TABLE Board_Users (
--    UserId INT IDENTITY(1,1) PRIMARY KEY,
--    UserName NVARCHAR(100) NOT NULL,
--    Email NVARCHAR(200) NOT NULL UNIQUE,
--    Password NVARCHAR(255) NOT NULL, -- 암호화된 비밀번호 저장
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL
--);

---- 게시글 (원글 + 답글 통합 구조)
--CREATE TABLE Board_Posts (
--    PostId INT IDENTITY(1,1) PRIMARY KEY,
--    UserId INT NOT NULL,
--    ParentPostId INT NULL, -- 답글인 경우 원글 참조
--    Title NVARCHAR(200) NOT NULL,
--    Content NVARCHAR(MAX) NOT NULL,
--    ViewCount INT NOT NULL DEFAULT 0,
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL,
--    FOREIGN KEY (UserId) REFERENCES Board_Users(UserId),
--    FOREIGN KEY (ParentPostId) REFERENCES Board_Posts(PostId)
--);

---- 댓글 테이블 (대댓글 가능)
--CREATE TABLE Board_Comments (
--    CommentId INT IDENTITY(1,1) PRIMARY KEY,
--    PostId INT NOT NULL,
--    UserId INT NOT NULL,
--    ParentCommentId INT NULL, -- 대댓글의 경우 사용
--    Content NVARCHAR(MAX) NOT NULL,
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    UpdatedAt DATETIME NULL,
--    FOREIGN KEY (PostId) REFERENCES Board_Posts(PostId),
--    FOREIGN KEY (UserId) REFERENCES Board_Users(UserId),
--    FOREIGN KEY (ParentCommentId) REFERENCES Board_Comments(CommentId)
--);

---- 좋아요 테이블 (게시글/댓글 구분 통합형)
--CREATE TABLE Board_Likes (
--    LikeId INT IDENTITY(1,1) PRIMARY KEY,
--    UserId INT NOT NULL,
--    TargetId INT NOT NULL, -- PostId 또는 CommentId
--    TargetType NVARCHAR(20) NOT NULL CHECK (TargetType IN ('Post', 'Comment')),
--    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
--    CONSTRAINT UQ_Board_Likes UNIQUE(UserId, TargetId, TargetType),
--    FOREIGN KEY (UserId) REFERENCES Board_Users(UserId)
--);

-----------------------------------------------

--ALTER TABLE dbo.Board_Comments
--ADD IsDeleted BIT NOT NULL DEFAULT 0;

-----------------------------------------------

-- 1~3번 UserId, 랜덤 제목/내용을 포함한 게시글 200개 삽입
DECLARE @i INT = 1;

-- 단어 리스트 테이블 변수 선언
DECLARE @WordTable TABLE (Id INT IDENTITY(1,1), Word NVARCHAR(50));
INSERT INTO @WordTable (Word)
VALUES 
    (N'고양이'), (N'강아지'), (N'AI'), (N'데이터'), (N'분석'), 
    (N'테스트'), (N'샘플'), (N'속도'), (N'기록'), (N'프로그래밍'),
    (N'자동화'), (N'보안'), (N'API'), (N'클라우드'), (N'챗봇');

-- 루프 삽입 시작
WHILE @i <= 200
BEGIN
    DECLARE @Random1 INT = FLOOR(RAND(CHECKSUM(NEWID())) * 15) + 1;
    DECLARE @Random2 INT = FLOOR(RAND(CHECKSUM(CONVERT(varchar(36), NEWID()) + '1')) * 15) + 1;
    DECLARE @Random3 INT = FLOOR(RAND(CHECKSUM(CONVERT(varchar(36), NEWID()) + '2')) * 15) + 1;
    DECLARE @Random4 INT = FLOOR(RAND(CHECKSUM(CONVERT(varchar(36), NEWID()) + '3')) * 15) + 1;

    DECLARE @Title NVARCHAR(200);
    DECLARE @Content NVARCHAR(MAX);
    DECLARE @UserId INT = FLOOR(RAND(CHECKSUM(NEWID())) * 3) + 1;
    DECLARE @ViewCount INT = FLOOR(RAND(CHECKSUM(NEWID())) * 100);

    -- 제목 조합
    SELECT @Title = CONCAT('글: ', w1.Word, ' ', w2.Word)
    FROM @WordTable w1, @WordTable w2
    WHERE w1.Id = @Random1 AND w2.Id = @Random2;

    -- 내용 조합
    SELECT @Content = CONCAT('내용: ', w3.Word, '와 ', w4.Word, '에 대한 설명입니다.')
    FROM @WordTable w3, @WordTable w4
    WHERE w3.Id = @Random3 AND w4.Id = @Random4;

    -- 게시글 삽입
    INSERT INTO dbo.Board_Posts (UserId, ParentPostId, Title, Content, ViewCount, CreatedAt, UpdatedAt)
    VALUES (
        @UserId,
        NULL,
        @Title,
        @Content,
        @ViewCount,
        GETDATE(),
        GETDATE()
    );

    SET @i += 1;
END
