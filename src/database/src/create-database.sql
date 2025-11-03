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
-- Скрипт создания базы данных для задачи TaskForge
-- =============================================
-- Описание: Создает базу данных с таблицей платежей клиентов
--           и табличной функцией для получения поденных сумм платежей
-- =============================================

USE master;
GO

-- Проверка существования базы данных
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'TaskForgeDB')
BEGIN
    ALTER DATABASE TaskForgeDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE TaskForgeDB;
END
GO

-- Создание базы данных
-- Примечание: Пути к файлам будут определены сервером автоматически
-- Если необходимо указать конкретные пути, раскомментируйте и измените FILENAME
CREATE DATABASE TaskForgeDB;
-- ALTER DATABASE TaskForgeDB MODIFY FILE (NAME = 'TaskForgeDB', SIZE = 100MB, MAXSIZE = 1GB, FILEGROWTH = 10MB);
-- ALTER DATABASE TaskForgeDB MODIFY FILE (NAME = 'TaskForgeDB_Log', SIZE = 10MB, MAXSIZE = 100MB, FILEGROWTH = 5MB);
GO

USE TaskForgeDB;
GO

-- =============================================
-- Создание схемы (если необходимо)
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = 'dbo')
BEGIN
    EXEC('CREATE SCHEMA dbo AUTHORIZATION [dbo]');
END
GO

PRINT 'Database TaskForgeDB created successfully.';
GO

