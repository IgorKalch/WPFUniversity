USE UniversityDb;
Go 
-- Generate and insert data into the Courses table
INSERT INTO Courses (Name, Description)
SELECT TOP 20
    'Course' + CAST(ROW_NUMBER() OVER (ORDER BY NEWID()) AS NVARCHAR(10)),
    'Description' + CAST(ROW_NUMBER() OVER (ORDER BY NEWID()) AS NVARCHAR(10))
FROM sys.objects;

-- Generate and insert data into the Teachers table
INSERT INTO Teachers (CourseId, FirstName, LastName, Subject)
SELECT TOP 20
    1,  -- Replace with valid CourseId
    'First' + CAST(ROW_NUMBER() OVER (ORDER BY NEWID()) AS NVARCHAR(10)),
    'Last' + CAST(ROW_NUMBER() OVER (ORDER BY NEWID()) AS NVARCHAR(10)),
    'Subject' + CAST(ROW_NUMBER() OVER (ORDER BY NEWID()) AS NVARCHAR(10))
FROM sys.objects;

-- Generate and insert data into the Groups table
INSERT INTO Groups (CourseId, TeacherId, Name)
SELECT TOP 20
    1,  -- Replace with valid CourseId
    1,  -- Replace with valid TeacherId
    'Group' + CAST(ROW_NUMBER() OVER (ORDER BY NEWID()) AS NVARCHAR(10))
FROM sys.objects;

-- Generate and insert data into the Students table
INSERT INTO Students (GroupId, FirstName, LastName)
SELECT TOP 20
    1,  -- Replace with valid GroupId
    'First' + CAST(ROW_NUMBER() OVER (ORDER BY NEWID()) AS NVARCHAR(10)),
    'Last' + CAST(ROW_NUMBER() OVER (ORDER BY NEWID()) AS NVARCHAR(10))
FROM sys.objects;