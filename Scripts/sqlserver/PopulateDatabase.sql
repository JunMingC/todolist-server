-- Create the Priorities table with Color column
CREATE TABLE Priorities (
    Id INT IDENTITY(1,1) PRIMARY KEY,           -- Unique identifier with auto-increment
    Name NVARCHAR(50) NOT NULL,					-- Name of the priority level (e.g., High, Medium, Low)
    Color CHAR(9) NOT NULL,						-- Color in hexadecimal format (e.g., #FF0000)
    CreatedAt DATETIME DEFAULT GETDATE(),       -- Timestamp for when the priority was created
    UpdatedAt DATETIME DEFAULT GETDATE()        -- Timestamp for when the priority was last updated
);

-- Insert initial data into the Priorities table
INSERT INTO Priorities (Name, Color) VALUES
('High', '#FF0000'),   -- Red
('Medium', '#FFFF00'), -- Yellow
('Low', '#00FF00');    -- Green

-- Create the Status table with Color column
CREATE TABLE Status (
    Id INT IDENTITY(1,1) PRIMARY KEY,           -- Unique identifier with auto-increment
    Name NVARCHAR(50) NOT NULL,					-- Name of the status (e.g., Not Started, In Progress, Completed)
    Color CHAR(9) NOT NULL,						-- Color in hexadecimal format (e.g., #FF0000)
    CreatedAt DATETIME DEFAULT GETDATE(),       -- Timestamp for when the status was created
    UpdatedAt DATETIME DEFAULT GETDATE()        -- Timestamp for when the status was last updated
);

-- Insert initial data into the Status table
INSERT INTO Status (Name, Color) VALUES
('On Hold', '#FF0000'),    -- Red
('In Progress', '#FFFF00'),    -- Yellow
('Completed', '#00FF00');      -- Green

-- Create the Tags table with Color column
CREATE TABLE Tags (
    Id INT IDENTITY(1,1) PRIMARY KEY,           -- Unique identifier with auto-increment
    Name NVARCHAR(50) NOT NULL,					-- Name of the tag
    Color CHAR(9) NOT NULL,						-- Color in hexadecimal format (e.g., #FF0000)
    CreatedAt DATETIME DEFAULT GETDATE(),       -- Timestamp for when the tag was created
    UpdatedAt DATETIME DEFAULT GETDATE()        -- Timestamp for when the tag was last updated
);

-- Insert initial data into the Tags table
INSERT INTO Tags (Name, Color) VALUES
('Personal', '#FF69B4'),    -- Hot Pink
('Work', '#4682B4'),        -- Steel Blue
('Research', '#8A2BE2'),    -- Blue Violet
('Development', '#32CD32'), -- Lime Green
('Review', '#FFD700'),      -- Gold
('Training', '#D3D3D3');    -- Light Gray

-- Create the Todos table with foreign key references to the Priorities and Status tables
CREATE TABLE Todos (
    Id INT IDENTITY(1,1) PRIMARY KEY,           -- Unique identifier with auto-increment
    Name NVARCHAR(255) NOT NULL,				-- Name of the TODO item
    Description NVARCHAR(MAX) NULL,             -- Description of the TODO item, can be NULL
    DueDate DATETIME NULL,                      -- Due date for the TODO item, can be NULL
    PriorityId INT NULL,                        -- Foreign key to the Priorities table, can be NULL
    StatusId INT NULL,                          -- Foreign key to the Status table, can be NULL
    CreatedAt DATETIME DEFAULT GETDATE(),       -- Timestamp for when the TODO was created
    UpdatedAt DATETIME DEFAULT GETDATE(),       -- Timestamp for when the TODO was last updated
    CONSTRAINT FK_Todos_Priorities FOREIGN KEY (PriorityId)
    REFERENCES Priorities(Id)                  -- Establish foreign key relationship with Priorities
    ON DELETE SET NULL,                        -- Set the PriorityId to NULL if the referenced record is deleted
    CONSTRAINT FK_Todos_Status FOREIGN KEY (StatusId)
    REFERENCES Status(Id)                       -- Establish foreign key relationship with Status
    ON DELETE SET NULL                         -- Set the StatusId to NULL if the referenced record is deleted
);

-- Create the TodoTags junction table
CREATE TABLE TodoTags (
    TodoId INT NOT NULL,                        -- Foreign key to the Todos table
    TagId INT NOT NULL,                         -- Foreign key to the Tags table
    PRIMARY KEY (TodoId, TagId),                -- Composite primary key
    CONSTRAINT FK_TodoTags_Todos FOREIGN KEY (TodoId)
    REFERENCES Todos(Id) ON DELETE CASCADE,     -- Foreign key to Todos with cascade delete
    CONSTRAINT FK_TodoTags_Tags FOREIGN KEY (TagId)
    REFERENCES Tags(Id) ON DELETE CASCADE       -- Foreign key to Tags with cascade delete
);

-- Insert initial data into the Todos table
INSERT INTO Todos (Name, Description, DueDate, PriorityId, StatusId) VALUES
('Prepare project proposal', 'Draft and finalize the project proposal for the new client.', '2024-08-31', 1, 2),
('Team meeting', 'Weekly sync-up with the team to discuss project status.', '2025-06-10', 3, 1),
('Code review', 'Review the code submitted by the development team.', '2024-09-03', 1, 2),
('Update documentation', 'Update the project documentation with the latest changes.', '2024-09-20', 1, 1),
('Client presentation', 'Present the project progress to the client.', '2024-08-27', 3, 1),
('Research new technologies', 'Explore new tools and frameworks that can be used in future projects.', '2024-07-31', 2, 3),
('Training session', 'Conduct training for new team members on the project.', '2024-10-03', 2, 3);

-- Insert initial data into the TodoTags table
INSERT INTO TodoTags (TodoId, TagId) VALUES
(1, 2),
(1, 3),
(1, 5),
(1, 6),
(2, 2),
(2, 6),
(3, 2),
(3, 3),
(3, 5),
(5, 1),
(5, 2),
(5, 3),
(5, 4),
(5, 5),
(5, 6),
(6, 6),
(7, 1),
(7, 2),
(7, 3),
(7, 4),
(7, 5);