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
-- Ожидаемые результаты выполнения функции
-- =============================================
-- Описание: Таблицы с ожидаемыми результатами для сравнения
-- =============================================

USE TaskForgeDB;
GO

-- =============================================
-- Ожидаемые результаты для примера 1
-- =============================================
-- ClientId = 1, Sd = 2022-01-02, Ed = 2022-01-07

IF OBJECT_ID('tempdb..#ExpectedResult1', 'U') IS NOT NULL
    DROP TABLE #ExpectedResult1;
GO

CREATE TABLE #ExpectedResult1
(
    Dt date NOT NULL,
    Сумма money NOT NULL
);

INSERT INTO #ExpectedResult1 (Dt, Сумма) VALUES
    ('2022-01-02', 0),
    ('2022-01-03', 100),
    ('2022-01-04', 0),
    ('2022-01-05', 450),
    ('2022-01-06', 0),
    ('2022-01-07', 50);

PRINT 'Ожидаемые результаты для примера 1:';
PRINT '----------------------------------------';
SELECT * FROM #ExpectedResult1 ORDER BY Dt;
GO

PRINT '';
PRINT 'Фактические результаты функции для примера 1:';
PRINT '----------------------------------------';
SELECT * FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07') ORDER BY Dt;
GO

PRINT '';
PRINT 'Сравнение результатов для примера 1:';
PRINT '----------------------------------------';
SELECT 
    COALESCE(e.Dt, f.Dt) AS Dt,
    e.Сумма AS Ожидается,
    f.Сумма AS Получено,
    CASE 
        WHEN e.Сумма = f.Сумма THEN '✓ Соответствует'
        WHEN e.Dt IS NULL THEN '✗ Отсутствует в ожидаемых'
        WHEN f.Dt IS NULL THEN '✗ Отсутствует в результате'
        ELSE '✗ Не соответствует'
    END AS Статус
FROM #ExpectedResult1 e
FULL OUTER JOIN dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07') f ON e.Dt = f.Dt
ORDER BY COALESCE(e.Dt, f.Dt);
GO

-- =============================================
-- Ожидаемые результаты для примера 2
-- =============================================
-- ClientId = 2, Sd = 2022-01-04, Ed = 2022-01-11

IF OBJECT_ID('tempdb..#ExpectedResult2', 'U') IS NOT NULL
    DROP TABLE #ExpectedResult2;
GO

CREATE TABLE #ExpectedResult2
(
    Dt date NOT NULL,
    Сумма money NOT NULL
);

INSERT INTO #ExpectedResult2 (Dt, Сумма) VALUES
    ('2022-01-04', 0),
    ('2022-01-05', 278),
    ('2022-01-06', 0),
    ('2022-01-07', 0),
    ('2022-01-08', 0),
    ('2022-01-09', 0),
    ('2022-01-10', 300),
    ('2022-01-11', 0);

PRINT '';
PRINT '========================================';
PRINT 'Ожидаемые результаты для примера 2:';
PRINT '----------------------------------------';
SELECT * FROM #ExpectedResult2 ORDER BY Dt;
GO

PRINT '';
PRINT 'Фактические результаты функции для примера 2:';
PRINT '----------------------------------------';
SELECT * FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11') ORDER BY Dt;
GO

PRINT '';
PRINT 'Сравнение результатов для примера 2:';
PRINT '----------------------------------------';
SELECT 
    COALESCE(e.Dt, f.Dt) AS Dt,
    e.Сумма AS Ожидается,
    f.Сумма AS Получено,
    CASE 
        WHEN e.Сумма = f.Сумма THEN '✓ Соответствует'
        WHEN e.Dt IS NULL THEN '✗ Отсутствует в ожидаемых'
        WHEN f.Dt IS NULL THEN '✗ Отсутствует в результате'
        ELSE '✗ Не соответствует'
    END AS Статус
FROM #ExpectedResult2 e
FULL OUTER JOIN dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11') f ON e.Dt = f.Dt
ORDER BY COALESCE(e.Dt, f.Dt);
GO

-- =============================================
-- Итоговая статистика
-- =============================================
PRINT '';
PRINT '========================================';
PRINT 'ИТОГОВАЯ СТАТИСТИКА';
PRINT '========================================';

SELECT 
    'Пример 1' AS Пример,
    COUNT(*) AS ВсегоДней,
    SUM(CASE WHEN e.Сумма = f.Сумма THEN 1 ELSE 0 END) AS Совпадений,
    SUM(CASE WHEN e.Сумма != f.Сумма OR e.Dt IS NULL OR f.Dt IS NULL THEN 1 ELSE 0 END) AS Несовпадений
FROM #ExpectedResult1 e
FULL OUTER JOIN dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07') f ON e.Dt = f.Dt

UNION ALL

SELECT 
    'Пример 2' AS Пример,
    COUNT(*) AS ВсегоДней,
    SUM(CASE WHEN e.Сумма = f.Сумма THEN 1 ELSE 0 END) AS Совпадений,
    SUM(CASE WHEN e.Сумма != f.Сумма OR e.Dt IS NULL OR f.Dt IS NULL THEN 1 ELSE 0 END) AS Несовпадений
FROM #ExpectedResult2 e
FULL OUTER JOIN dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11') f ON e.Dt = f.Dt;

GO

-- Очистка временных таблиц
IF OBJECT_ID('tempdb..#ExpectedResult1', 'U') IS NOT NULL
    DROP TABLE #ExpectedResult1;

IF OBJECT_ID('tempdb..#ExpectedResult2', 'U') IS NOT NULL
    DROP TABLE #ExpectedResult2;
GO

PRINT '';
PRINT '========================================';
PRINT 'Проверка завершена.';
PRINT '========================================';
GO

