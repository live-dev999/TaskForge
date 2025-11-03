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
-- Полный вывод результатов выполнения задания
-- =============================================
-- Описание: Полная демонстрация работы функции с форматированным выводом
--           Используется для презентации результатов выполнения задания
-- =============================================

USE TaskForgeDB;
GO

SET NOCOUNT ON;
GO

PRINT '════════════════════════════════════════════════════════════════';
PRINT '           РЕЗУЛЬТАТЫ ВЫПОЛНЕНИЯ ЗАДАНИЯ';
PRINT '           Табличная функция GetDailyPayments';
PRINT '════════════════════════════════════════════════════════════════';
PRINT '';
GO

-- =============================================
-- ПРИМЕР 1
-- =============================================

PRINT '╔═══════════════════════════════════════════════════════════════╗';
PRINT '║                    ПРИМЕР 1                                   ║';
PRINT '╚═══════════════════════════════════════════════════════════════╝';
PRINT '';
PRINT 'Входные данные:';
PRINT '  • ClientId = 1';
PRINT '  • Sd = 2022-01-02';
PRINT '  • Ed = 2022-01-07';
PRINT '';
PRINT 'Результат работы функции:';
PRINT '';
PRINT '┌────────────┬──────────┐';
PRINT '│     Dt     │  Сумма  │';
PRINT '├────────────┼──────────┤';

-- Форматированный вывод результата
DECLARE @Result1 TABLE (Dt date, Сумма money);
INSERT INTO @Result1
SELECT Dt, Сумма
FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
ORDER BY Dt;

DECLARE @dt1 date, @sum1 money;
DECLARE result_cursor CURSOR FOR SELECT Dt, Сумма FROM @Result1 ORDER BY Dt;
OPEN result_cursor;
FETCH NEXT FROM result_cursor INTO @dt1, @sum1;
WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT '│ ' + FORMAT(@dt1, 'yyyy-MM-dd') + ' │ ' + 
          RIGHT('      ' + CAST(@sum1 AS varchar(20)), 8) + ' │';
    FETCH NEXT FROM result_cursor INTO @dt1, @sum1;
END
CLOSE result_cursor;
DEALLOCATE result_cursor;

PRINT '└────────────┴──────────┘';
PRINT '';
GO

-- =============================================
-- ПРИМЕР 2
-- =============================================

PRINT '╔═══════════════════════════════════════════════════════════════╗';
PRINT '║                    ПРИМЕР 2                                   ║';
PRINT '╚═══════════════════════════════════════════════════════════════╝';
PRINT '';
PRINT 'Входные данные:';
PRINT '  • ClientId = 2';
PRINT '  • Sd = 2022-01-04';
PRINT '  • Ed = 2022-01-11';
PRINT '';
PRINT 'Результат работы функции:';
PRINT '';
PRINT '┌────────────┬──────────┐';
PRINT '│     Dt     │  Сумма  │';
PRINT '├────────────┼──────────┤';

DECLARE @Result2 TABLE (Dt date, Сумма money);
INSERT INTO @Result2
SELECT Dt, Сумма
FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')
ORDER BY Dt;

DECLARE @dt2 date, @sum2 money;
DECLARE result_cursor2 CURSOR FOR SELECT Dt, Сумма FROM @Result2 ORDER BY Dt;
OPEN result_cursor2;
FETCH NEXT FROM result_cursor2 INTO @dt2, @sum2;
WHILE @@FETCH_STATUS = 0
BEGIN
    PRINT '│ ' + FORMAT(@dt2, 'yyyy-MM-dd') + ' │ ' + 
          RIGHT('      ' + CAST(@sum2 AS varchar(20)), 8) + ' │';
    FETCH NEXT FROM result_cursor2 INTO @dt2, @sum2;
END
CLOSE result_cursor2;
DEALLOCATE result_cursor2;

PRINT '└────────────┴──────────┘';
PRINT '';
GO

-- =============================================
-- Проверка соответствия ожидаемым результатам
-- =============================================

PRINT '╔═══════════════════════════════════════════════════════════════╗';
PRINT '║         ПРОВЕРКА СООТВЕТСТВИЯ ОЖИДАЕМЫМ РЕЗУЛЬТАТАМ           ║';
PRINT '╚═══════════════════════════════════════════════════════════════╝';
PRINT '';

-- Проверка примера 1
DECLARE @Example1Valid bit = 1;

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
    FULL OUTER JOIN dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07') fr ON er.Dt = fr.Dt
    WHERE er.ExpectedSum != fr.Сумма OR er.Dt IS NULL OR fr.Dt IS NULL
)
    SET @Example1Valid = 0;

-- Проверка примера 2
DECLARE @Example2Valid bit = 1;

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
    FULL OUTER JOIN dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11') fr ON er.Dt = fr.Dt
    WHERE er.ExpectedSum != fr.Сумма OR er.Dt IS NULL OR fr.Dt IS NULL
)
    SET @Example2Valid = 0;

PRINT 'Пример 1 (ClientId=1, 2022-01-02 to 2022-01-07): ' + 
      CASE WHEN @Example1Valid = 1 THEN '✓ ПРОЙДЕН' ELSE '✗ НЕ ПРОЙДЕН' END;
PRINT 'Пример 2 (ClientId=2, 2022-01-04 to 2022-01-11): ' + 
      CASE WHEN @Example2Valid = 1 THEN '✓ ПРОЙДЕН' ELSE '✗ НЕ ПРОЙДЕН' END;
PRINT '';

IF @Example1Valid = 1 AND @Example2Valid = 1
BEGIN
    PRINT '════════════════════════════════════════════════════════════════';
    PRINT '✓ ЗАДАНИЕ ВЫПОЛНЕНО УСПЕШНО!';
    PRINT 'Все результаты соответствуют ожидаемым значениям.';
    PRINT '════════════════════════════════════════════════════════════════';
END
ELSE
BEGIN
    PRINT '════════════════════════════════════════════════════════════════';
    PRINT '✗ ОБНАРУЖЕНЫ НЕСООТВЕТСТВИЯ!';
    PRINT 'Необходимо проверить реализацию функции.';
    PRINT '════════════════════════════════════════════════════════════════';
END
GO

