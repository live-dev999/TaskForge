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
-- Тесты для функции GetDailyPayments
-- =============================================
-- Описание: Тестовые запросы для проверки работы функции
-- =============================================

USE TaskForgeDB;
GO

PRINT '========================================';
PRINT 'Testing function GetDailyPayments';
PRINT '========================================';
GO

-- =============================================
-- Тест 1: ClientId = 1, период 2022-01-02 до 2022-01-07
-- Ожидаемый результат:
--   2022-01-02 | 0
--   2022-01-03 | 100
--   2022-01-04 | 0
--   2022-01-05 | 450
--   2022-01-06 | 0
--   2022-01-07 | 50
-- =============================================
PRINT '';
PRINT 'Test 1: ClientId = 1, period 2022-01-02 to 2022-01-07';
PRINT '----------------------------------------';
SELECT * 
FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
ORDER BY Dt;
GO

-- =============================================
-- Тест 2: ClientId = 2, период 2022-01-04 до 2022-01-11
-- Ожидаемый результат:
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
PRINT 'Test 2: ClientId = 2, period 2022-01-04 to 2022-01-11';
PRINT '----------------------------------------';
SELECT * 
FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')
ORDER BY Dt;
GO

-- =============================================
-- Тест 3: Клиент без платежей (несуществующий ClientId)
-- =============================================
PRINT '';
PRINT 'Test 3: ClientId = 999 (non-existent client)';
PRINT '----------------------------------------';
SELECT * 
FROM dbo.GetDailyPayments(999, '2022-01-01', '2022-01-05')
ORDER BY Dt;
GO

-- =============================================
-- Тест 4: Один день (начальная и конечная даты совпадают)
-- =============================================
PRINT '';
PRINT 'Test 4: Single day (Sd = Ed)';
PRINT '----------------------------------------';
SELECT * 
FROM dbo.GetDailyPayments(1, '2022-01-05', '2022-01-05')
ORDER BY Dt;
GO

-- =============================================
-- Тест 5: Долгий период (проверка работы с несколькими годами)
-- =============================================
PRINT '';
PRINT 'Test 5: Long period across years';
PRINT '----------------------------------------';
SELECT 
    Dt,
    Сумма,
    CASE 
        WHEN Сумма > 0 THEN 'Has payments'
        ELSE 'No payments'
    END AS Status
FROM dbo.GetDailyPayments(1, '2022-01-01', '2022-01-31')
ORDER BY Dt;
GO

-- =============================================
-- Тест 6: Проверка суммы всех платежей за период
-- =============================================
PRINT '';
PRINT 'Test 6: Total amount validation';
PRINT '----------------------------------------';
SELECT 
    ClientId,
    SUM(Сумма) AS TotalAmount,
    COUNT(*) AS DaysWithPayments,
    COUNT(*) - SUM(CASE WHEN Сумма > 0 THEN 1 ELSE 0 END) AS DaysWithoutPayments
FROM dbo.GetDailyPayments(1, '2022-01-01', '2022-01-31')
GROUP BY ClientId;
GO

PRINT '';
PRINT '========================================';
PRINT 'All tests completed.';
PRINT '========================================';
GO

