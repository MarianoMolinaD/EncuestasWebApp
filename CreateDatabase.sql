CREATE DATABASE EncuestasDB;
GO

USE EncuestasDB;
GO

-- USUARIOS
CREATE TABLE Users (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    UserName NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(200) NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE()
);
GO

-- ENCUESTAS
CREATE TABLE Surveys (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    [Name] NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    UniqueLink NVARCHAR(100) NOT NULL UNIQUE,
    CreatedAt DATETIME DEFAULT GETDATE(),
    UserId INT NOT NULL,
	Deleted CHAR(1) NOT NULL DEFAULT 'N',
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

-- CAMPOS ENCUESTA
CREATE TABLE SurveyFields (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SurveyId INT NOT NULL,
    FieldName NVARCHAR(100) NOT NULL,
    FieldType NVARCHAR(20) NOT NULL CHECK (FieldType IN ('Text', 'Number', 'Date')),
    IsRequired BIT DEFAULT 0,
	FieldTitle NVARCHAR(200) NOT NULL,
    FOREIGN KEY (SurveyId) REFERENCES Surveys(Id) ON DELETE CASCADE
);
GO

-- RESPUESTAS
CREATE TABLE Responses (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    SurveyId INT NOT NULL,
    CreatedAt DATETIME DEFAULT GETDATE(),
    FOREIGN KEY (SurveyId) REFERENCES Surveys(Id) ON DELETE CASCADE
);
GO

-- DETALLE RESPUESTAS
CREATE TABLE ResponseDetails (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    ResponseId INT NOT NULL,
    FieldId INT NOT NULL,
    [Value] NVARCHAR(MAX),
    FOREIGN KEY (ResponseId) REFERENCES Responses(Id) ,
    FOREIGN KEY (FieldId) REFERENCES SurveyFields(Id) 
);
GO

-- INDICES
CREATE INDEX IX_Surveys_User ON Surveys(UserId);
CREATE INDEX IX_SurveyFields_Survey ON SurveyFields(SurveyId);
CREATE INDEX IX_Responses_Survey ON Responses(SurveyId);
CREATE INDEX IX_ResponseDetails_Response ON ResponseDetails(ResponseId);
GO

-- DEFAULT ADMIN USER 
-- USUARIO: Admin.
-- PASSWORD: Admin.
INSERT INTO Users (UserName, PasswordHash)
VALUES ('Admin', '$2a$10$/4Z6tG5jm3ZSQ0hlH7lFV.J3xg0L6vXaLeD5QbVgqvyiEG1wmbCHq');
GO
