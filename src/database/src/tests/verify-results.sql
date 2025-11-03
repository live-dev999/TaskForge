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
-- Скрипт проверки результатов выполнения задания
-- =============================================
-- Описание: Автоматическая проверка соответствия результатов
--           функции ожидаемым значениям из задания
-- =============================================

USE TaskForgeDB;
GO

SET NOCOUNT ON;
GO

PRINT '========================================';
PRINT 'Проверка результатов выполнения задания';
PRINT '========================================';
PRINT '';
GO

DECLARE @AllTestsPassed bit = 1;
DECLARE @TestCount int = 0;
DECLARE @PassedCount int = 0;

-- =============================================
-- Тест 1: Пример из задания №1
-- =============================================
SET @TestCount = @TestCount + 1;
PRINT 'Тест ' + CAST(@TestCount AS varchar(10)) + ': Пример 1 (ClientId=1, 2022-01-02 to 2022-01-07)';

DECLARE @Test1Passed bit = 1;

-- Проверяем все ожидаемые результаты
IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
    WHERE Dt = '2022-01-02' AND Сумма = 0
)
    SET @Test1Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
    WHERE Dt = '2022-01-03' AND Сумма = 100
)
    SET @Test1Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
    WHERE Dt = '2022-01-04' AND Сумма = 0
)
    SET @Test1Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
    WHERE Dt = '2022-01-05' AND Сумма = 450
)
    SET @Test1Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
    WHERE Dt = '2022-01-06' AND Сумма = 0
)
    SET @Test1Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
    WHERE Dt = '2022-01-07' AND Сумма = 50
)
    SET @Test1Passed = 0;

-- Проверяем количество строк (должно быть 6)
IF (SELECT COUNT(*) FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')) != 6
    SET @Test1Passed = 0;

IF @Test1Passed = 1
BEGIN
    SET @PassedCount = @PassedCount + 1;
    PRINT '  Результат: ✓ ПРОЙДЕН';
END
ELSE
BEGIN
    SET @AllTestsPassed = 0;
    PRINT '  Результат: ✗ НЕ ПРОЙДЕН';
END
GO

-- =============================================
-- Тест 2: Пример из задания №2
-- =============================================
DECLARE @Test2Passed bit = 1;
SET @TestCount = @TestCount + 1;
PRINT '';
PRINT 'Тест ' + CAST(@TestCount AS varchar(10)) + ': Пример 2 (ClientId=2, 2022-01-04 to 2022-01-11)';

-- Проверяем все ожидаемые результаты
IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')
    WHERE Dt = '2022-01-04' AND Сумма = 0
)
    SET @Test2Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')
    WHERE Dt = '2022-01-05' AND Сумма = 278
)
    SET @Test2Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')
    WHERE Dt = '2022-01-06' AND Сумма = 0
)
    SET @Test2Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')
    WHERE Dt = '2022-01-07' AND Сумма = 0
)
    SET @Test2Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')
    WHERE Dt = '2022-01-08' AND Сумма = 0
)
    SET @Test2Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')
    WHERE Dt = '2022-01-09' AND Сумма = 0
)
    SET @Test2Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')
    WHERE Dt = '2022-01-10' AND Сумма = 300
)
    SET @Test2Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')
    WHERE Dt = '2022-01-11' AND Сумма = 0
)
    SET @Test2Passed = 0;

-- Проверяем количество строк (должно быть 8)
IF (SELECT COUNT(*) FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')) != 8
    SET @Test2Passed = 0;

IF @Test2Passed = 1
BEGIN
    SET @PassedCount = @PassedCount + 1;
    PRINT '  Результат: ✓ ПРОЙДЕН';
END
ELSE
BEGIN
    SET @AllTestsPassed = 0;
    PRINT '  Результат: ✗ НЕ ПРОЙДЕН';
END
GO

-- =============================================
-- Тест 3: Проверка возврата нулей для дней без платежей
-- =============================================
SET @TestCount = @TestCount + 1;
PRINT '';
PRINT 'Тест ' + CAST(@TestCount AS varchar(10)) + ': Проверка нулей для дней без платежей';

DECLARE @Test3Passed bit = 1;

-- Проверяем, что для дней без платежей возвращается 0, а не NULL
IF EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
    WHERE Сумма IS NULL
)
    SET @Test3Passed = 0;

-- Проверяем конкретные дни с нулевыми суммами
IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
    WHERE Dt = '2022-01-02' AND Сумма = 0
)
    SET @Test3Passed = 0;

IF @Test3Passed = 1
BEGIN
    SET @PassedCount = @PassedCount + 1;
    PRINT '  Результат: ✓ ПРОЙДЕН';
END
ELSE
BEGIN
    SET @AllTestsPassed = 0;
    PRINT '  Результат: ✗ НЕ ПРОЙДЕН';
END
GO

-- =============================================
-- Тест 4: Проверка включения граничных дат
-- =============================================
SET @TestCount = @TestCount + 1;
PRINT '';
PRINT 'Тест ' + CAST(@TestCount AS varchar(10)) + ': Проверка включения граничных дат';

DECLARE @Test4Passed bit = 1;

-- Проверяем, что начальная и конечная даты включены в результат
IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
    WHERE Dt = '2022-01-02'
)
    SET @Test4Passed = 0;

IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
    WHERE Dt = '2022-01-07'
)
    SET @Test4Passed = 0;

IF @Test4Passed = 1
BEGIN
    SET @PassedCount = @PassedCount + 1;
    PRINT '  Результат: ✓ ПРОЙДЕН';
END
ELSE
BEGIN
    SET @AllTestsPassed = 0;
    PRINT '  Результат: ✗ НЕ ПРОЙДЕН';
END
GO

-- =============================================
-- Тест 5: Проверка корректности суммирования платежей за день
-- =============================================
SET @TestCount = @TestCount + 1;
PRINT '';
PRINT 'Тест ' + CAST(@TestCount AS varchar(10)) + ': Проверка суммирования платежей за день';

DECLARE @Test5Passed bit = 1;

-- Добавляем дополнительные платежи для проверки суммирования
INSERT INTO dbo.ClientPayments (ClientId, Dt, Amount) VALUES
    (3, '2022-01-10 10:00:00', 100.00),
    (3, '2022-01-10 14:30:00', 200.00),
    (3, '2022-01-10 16:45:00', 50.00);
GO

-- Проверяем, что все платежи за день суммируются
IF NOT EXISTS (
    SELECT 1
    FROM dbo.GetDailyPayments(3, '2022-01-10', '2022-01-10')
    WHERE Dt = '2022-01-10' AND Сумма = 350.00
)
    SET @Test5Passed = 0;

-- Удаляем тестовые данные
DELETE FROM dbo.ClientPayments WHERE ClientId = 3;
GO

IF @Test5Passed = 1
BEGIN
    SET @PassedCount = @PassedCount + 1;
    PRINT '  Результат: ✓ ПРОЙДЕН';
END
ELSE
BEGIN
    SET @AllTestsPassed = 0;
    PRINT '  Результат: ✗ НЕ ПРОЙДЕН';
END
GO

-- =============================================
-- Итоговый отчет
-- =============================================
PRINT '';
PRINT '========================================';
PRINT 'ИТОГОВЫЙ ОТЧЕТ';
PRINT '========================================';
PRINT 'Всего тестов: ' + CAST(@TestCount AS varchar(10));
PRINT 'Пройдено: ' + CAST(@PassedCount AS varchar(10));
PRINT 'Провалено: ' + CAST((@TestCount - @PassedCount) AS varchar(10));
PRINT '';

IF @AllTestsPassed = 1
BEGIN
    PRINT '✓ ВСЕ ТЕСТЫ ПРОЙДЕНЫ УСПЕШНО!';
    PRINT 'Функция GetDailyPayments работает корректно согласно заданию.';
END
ELSE
BEGIN
    PRINT '✗ ОБНАРУЖЕНЫ ОШИБКИ!';
    PRINT 'Необходимо проверить реализацию функции.';
END
GO

PRINT '';
PRINT '========================================';
GO

