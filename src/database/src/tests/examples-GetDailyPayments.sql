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
-- Примеры выполнения функции GetDailyPayments
-- =============================================
-- Описание: Результаты работы функции согласно заданию
-- =============================================

USE TaskForgeDB;
GO

PRINT '========================================';
PRINT 'Примеры выполнения функции GetDailyPayments';
PRINT '========================================';
GO

-- =============================================
-- Пример 1: Результат работы функции №1
-- =============================================
-- Входные данные:
--   ClientId = 1
--   Sd = 2022-01-02
--   Ed = 2022-01-07
-- 
-- Ожидаемый результат функции:
--   Dt         | Сумма
--   -----------|-------
--   2022-01-02 | 0
--   2022-01-03 | 100
--   2022-01-04 | 0
--   2022-01-05 | 450
--   2022-01-06 | 0
--   2022-01-07 | 50
-- =============================================

PRINT '';
PRINT 'Пример 1: ClientId = 1, период 2022-01-02 до 2022-01-07';
PRINT '----------------------------------------';
PRINT 'Входные данные:';
PRINT '  ClientId = 1';
PRINT '  Sd = 2022-01-02';
PRINT '  Ed = 2022-01-07';
PRINT '';
PRINT 'Результат функции:';
PRINT '';

SELECT 
    Dt,
    Сумма
FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
ORDER BY Dt;
GO

-- Проверка ожидаемого результата
PRINT '';
PRINT 'Проверка соответствия ожидаемому результату:';
WITH ExpectedResults AS
(
    SELECT CAST('2022-01-02' AS date) AS Dt, CAST(0 AS money) AS ExpectedSum
    UNION ALL SELECT CAST('2022-01-03' AS date), CAST(100 AS money)
    UNION ALL SELECT CAST('2022-01-04' AS date), CAST(0 AS money)
    UNION ALL SELECT CAST('2022-01-05' AS date), CAST(450 AS money)
    UNION ALL SELECT CAST('2022-01-06' AS date), CAST(0 AS money)
    UNION ALL SELECT CAST('2022-01-07' AS date), CAST(50 AS money)
)
SELECT 
    er.Dt,
    er.ExpectedSum AS Ожидается,
    fr.Сумма AS Получено,
    CASE 
        WHEN er.ExpectedSum = fr.Сумма THEN '✓ Соответствует'
        ELSE '✗ Не соответствует'
    END AS Статус
FROM ExpectedResults er
LEFT JOIN dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07') fr ON er.Dt = fr.Dt
ORDER BY er.Dt;
GO

-- =============================================
-- Пример 2: Результат работы функции №2
-- =============================================
-- Входные данные:
--   ClientId = 2
--   Sd = 2022-01-04
--   Ed = 2022-01-11
-- 
-- Ожидаемый результат функции:
--   Dt         | Сумма
--   -----------|-------
--   2022-01-04 | 0
--   2022-01-05 | 278
--   2022-01-06 | 0
--   2022-01-07 | 0
--   2022-01-08 | 0
--   2022-01-09 | 0
--   2022-01-10 | 300
--   2022-01-11 | 0
-- =============================================

PRINT '';
PRINT 'Пример 2: ClientId = 2, период 2022-01-04 до 2022-01-11';
PRINT '----------------------------------------';
PRINT 'Входные данные:';
PRINT '  ClientId = 2';
PRINT '  Sd = 2022-01-04';
PRINT '  Ed = 2022-01-11';
PRINT '';
PRINT 'Результат функции:';
PRINT '';

SELECT 
    Dt,
    Сумма
FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')
ORDER BY Dt;
GO

-- Проверка ожидаемого результата
PRINT '';
PRINT 'Проверка соответствия ожидаемому результату:';
WITH ExpectedResults AS
(
    SELECT CAST('2022-01-04' AS date) AS Dt, CAST(0 AS money) AS ExpectedSum
    UNION ALL SELECT CAST('2022-01-05' AS date), CAST(278 AS money)
    UNION ALL SELECT CAST('2022-01-06' AS date), CAST(0 AS money)
    UNION ALL SELECT CAST('2022-01-07' AS date), CAST(0 AS money)
    UNION ALL SELECT CAST('2022-01-08' AS date), CAST(0 AS money)
    UNION ALL SELECT CAST('2022-01-09' AS date), CAST(0 AS money)
    UNION ALL SELECT CAST('2022-01-10' AS date), CAST(300 AS money)
    UNION ALL SELECT CAST('2022-01-11' AS date), CAST(0 AS money)
)
SELECT 
    er.Dt,
    er.ExpectedSum AS Ожидается,
    fr.Сумма AS Получено,
    CASE 
        WHEN er.ExpectedSum = fr.Сумма THEN '✓ Соответствует'
        ELSE '✗ Не соответствует'
    END AS Статус
FROM ExpectedResults er
LEFT JOIN dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11') fr ON er.Dt = fr.Dt
ORDER BY er.Dt;
GO

-- =============================================
-- Итоговая проверка всех примеров
-- =============================================
PRINT '';
PRINT '========================================';
PRINT 'Итоговая проверка всех примеров';
PRINT '========================================';

DECLARE @Example1Valid bit = 1;
DECLARE @Example2Valid bit = 1;

-- Проверка примера 1
IF EXISTS (
    SELECT 1
    FROM (
        SELECT CAST('2022-01-02' AS date) AS Dt, CAST(0 AS money) AS ExpectedSum
        UNION ALL SELECT CAST('2022-01-03' AS date), CAST(100 AS money)
        UNION ALL SELECT CAST('2022-01-04' AS date), CAST(0 AS money)
        UNION ALL SELECT CAST('2022-01-05' AS date), CAST(450 AS money)
        UNION ALL SELECT CAST('2022-01-06' AS date), CAST(0 AS money)
        UNION ALL SELECT CAST('2022-01-07' AS date), CAST(50 AS money)
    ) er
    LEFT JOIN dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07') fr ON er.Dt = fr.Dt
    WHERE er.ExpectedSum != fr.Сумма OR fr.Dt IS NULL
)
SET @Example1Valid = 0;

-- Проверка примера 2
IF EXISTS (
    SELECT 1
    FROM (
        SELECT CAST('2022-01-04' AS date) AS Dt, CAST(0 AS money) AS ExpectedSum
        UNION ALL SELECT CAST('2022-01-05' AS date), CAST(278 AS money)
        UNION ALL SELECT CAST('2022-01-06' AS date), CAST(0 AS money)
        UNION ALL SELECT CAST('2022-01-07' AS date), CAST(0 AS money)
        UNION ALL SELECT CAST('2022-01-08' AS date), CAST(0 AS money)
        UNION ALL SELECT CAST('2022-01-09' AS date), CAST(0 AS money)
        UNION ALL SELECT CAST('2022-01-10' AS date), CAST(300 AS money)
        UNION ALL SELECT CAST('2022-01-11' AS date), CAST(0 AS money)
    ) er
    LEFT JOIN dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11') fr ON er.Dt = fr.Dt
    WHERE er.ExpectedSum != fr.Сумма OR fr.Dt IS NULL
)
SET @Example2Valid = 0;

SELECT 
    'Пример 1 (ClientId=1, 2022-01-02 to 2022-01-07)' AS Тест,
    CASE WHEN @Example1Valid = 1 THEN '✓ ПРОЙДЕН' ELSE '✗ НЕ ПРОЙДЕН' END AS Результат
UNION ALL
SELECT 
    'Пример 2 (ClientId=2, 2022-01-04 to 2022-01-11)' AS Тест,
    CASE WHEN @Example2Valid = 1 THEN '✓ ПРОЙДЕН' ELSE '✗ НЕ ПРОЙДЕН' END AS Результат;

IF @Example1Valid = 1 AND @Example2Valid = 1
BEGIN
    PRINT '';
    PRINT '✓ Все примеры из задания выполнены корректно!';
END
ELSE
BEGIN
    PRINT '';
    PRINT '✗ Обнаружены несоответствия в результатах!';
END
GO

PRINT '';
PRINT '========================================';
PRINT 'Примеры выполнены.';
PRINT '========================================';
GO

