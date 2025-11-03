/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.
 *
 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 *
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 *
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 */

-- =============================================
-- Главный скрипт установки базы данных
-- =============================================
-- Описание: Выполняет все скрипты создания базы данных,
--           таблиц, функций и тестовых данных
-- =============================================

SET NOCOUNT ON;
GO

PRINT '========================================';
PRINT 'Starting database installation...';
PRINT '========================================';
PRINT '';
GO

-- =============================================
-- Шаг 1: Создание базы данных
-- =============================================
PRINT 'Step 1: Creating database...';
USE master;
GO

IF EXISTS (SELECT name FROM sys.databases WHERE name = 'TaskForgeDB')
BEGIN
    ALTER DATABASE TaskForgeDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TaskForgeDB;
END
GO

CREATE DATABASE TaskForgeDB;
GO

USE TaskForgeDB;
GO

-- =============================================
-- Шаг 2: Создание таблицы ClientPayments
-- =============================================
PRINT '';
PRINT 'Step 2: Creating table ClientPayments...';
GO

IF OBJECT_ID('dbo.ClientPayments', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.ClientPayments;
END
GO

CREATE TABLE dbo.ClientPayments
(
    Id bigint IDENTITY(1,1) NOT NULL,
    ClientId bigint NOT NULL,
    Dt datetime2(0) NOT NULL,
    Amount money NOT NULL,
    CONSTRAINT PK_ClientPayments PRIMARY KEY CLUSTERED (Id ASC)
);
GO

CREATE NONCLUSTERED INDEX IX_ClientPayments_ClientId_Dt
ON dbo.ClientPayments (ClientId ASC, Dt ASC)
INCLUDE (Amount);
GO

CREATE NONCLUSTERED INDEX IX_ClientPayments_Dt
ON dbo.ClientPayments (Dt ASC);
GO

PRINT 'Table dbo.ClientPayments created successfully.';
GO

-- =============================================
-- Шаг 3: Создание функции GetDailyPayments
-- =============================================
PRINT '';
PRINT 'Step 3: Creating function GetDailyPayments...';
GO

IF OBJECT_ID('dbo.GetDailyPayments', 'TF') IS NOT NULL
BEGIN
    DROP FUNCTION dbo.GetDailyPayments;
END
GO

CREATE FUNCTION dbo.GetDailyPayments
(
    @ClientId bigint,
    @Sd date,
    @Ed date
)
RETURNS TABLE
AS
RETURN
(
    WITH DateRange AS
    (
        SELECT CAST(@Sd AS date) AS Dt
        UNION ALL
        SELECT DATEADD(DAY, 1, Dt) AS Dt
        FROM DateRange
        WHERE Dt < @Ed
    )
    SELECT 
        dr.Dt,
        ISNULL(SUM(cp.Amount), 0) AS Сумма
    FROM DateRange dr
    LEFT JOIN dbo.ClientPayments cp 
        ON cp.ClientId = @ClientId 
        AND CAST(cp.Dt AS date) = dr.Dt
    GROUP BY dr.Dt
    OPTION (MAXRECURSION 10000)
);
GO

PRINT 'Function dbo.GetDailyPayments created successfully.';
GO

-- =============================================
-- Шаг 4: Вставка тестовых данных
-- =============================================
PRINT '';
PRINT 'Step 4: Inserting test data...';
GO

INSERT INTO dbo.ClientPayments (ClientId, Dt, Amount) VALUES
    (1, '2022-01-03 10:00:00', 100.00),
    (1, '2022-01-05 14:30:00', 450.00),
    (1, '2022-01-07 09:15:00', 50.00),
    (2, '2022-01-05 11:20:00', 278.00),
    (2, '2022-01-10 16:45:00', 300.00);
GO

PRINT 'Test data inserted successfully. Total records: ' + CAST(@@ROWCOUNT AS varchar(10));
GO

PRINT '';
PRINT '========================================';
PRINT 'Database installation completed successfully!';
PRINT '========================================';
GO

