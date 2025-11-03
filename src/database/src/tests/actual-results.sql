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
-- Фактические результаты выполнения функции GetDailyPayments
-- =============================================
-- Описание: Результаты выполнения функции для примеров из задания
--           Эти результаты получены после выполнения функции в базе данных
-- =============================================

USE TaskForgeDB;
GO

SET NOCOUNT ON;
GO

PRINT '========================================';
PRINT 'Фактические результаты выполнения функции';
PRINT '========================================';
PRINT '';
GO

-- =============================================
-- Результат работы функции №1
-- =============================================
-- Входные данные:
--   ClientId = 1
--   Sd = 2022-01-02
--   Ed = 2022-01-07
-- =============================================

PRINT 'Результат работы функции №1:';
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

-- Сохранение результата в таблицу для справки
IF OBJECT_ID('tempdb..#Result1', 'U') IS NOT NULL
    DROP TABLE #Result1;

SELECT 
    Dt,
    Сумма
INTO #Result1
FROM dbo.GetDailyPayments(1, '2022-01-02', '2022-01-07')
ORDER BY Dt;

PRINT '';
PRINT 'Количество записей: ' + CAST((SELECT COUNT(*) FROM #Result1) AS varchar(10));
PRINT 'Общая сумма: ' + CAST((SELECT SUM(Сумма) FROM #Result1) AS varchar(20));
PRINT 'Дней с платежами: ' + CAST((SELECT COUNT(*) FROM #Result1 WHERE Сумма > 0) AS varchar(10));
PRINT 'Дней без платежей: ' + CAST((SELECT COUNT(*) FROM #Result1 WHERE Сумма = 0) AS varchar(10));
GO

-- =============================================
-- Результат работы функции №2
-- =============================================
-- Входные данные:
--   ClientId = 2
--   Sd = 2022-01-04
--   Ed = 2022-01-11
-- =============================================

PRINT '';
PRINT '========================================';
PRINT 'Результат работы функции №2:';
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

-- Сохранение результата в таблицу для справки
IF OBJECT_ID('tempdb..#Result2', 'U') IS NOT NULL
    DROP TABLE #Result2;

SELECT 
    Dt,
    Сумма
INTO #Result2
FROM dbo.GetDailyPayments(2, '2022-01-04', '2022-01-11')
ORDER BY Dt;

PRINT '';
PRINT 'Количество записей: ' + CAST((SELECT COUNT(*) FROM #Result2) AS varchar(10));
PRINT 'Общая сумма: ' + CAST((SELECT SUM(Сумма) FROM #Result2) AS varchar(20));
PRINT 'Дней с платежами: ' + CAST((SELECT COUNT(*) FROM #Result2 WHERE Сумма > 0) AS varchar(10));
PRINT 'Дней без платежей: ' + CAST((SELECT COUNT(*) FROM #Result2 WHERE Сумма = 0) AS varchar(10));
GO

-- =============================================
-- Итоговая сводка результатов
-- =============================================

PRINT '';
PRINT '========================================';
PRINT 'ИТОГОВАЯ СВОДКА РЕЗУЛЬТАТОВ';
PRINT '========================================';

SELECT 
    'Пример 1' AS Пример,
    'ClientId=1, 2022-01-02 to 2022-01-07' AS Параметры,
    COUNT(*) AS ВсегоДней,
    SUM(Сумма) AS ОбщаяСумма,
    SUM(CASE WHEN Сумма > 0 THEN 1 ELSE 0 END) AS ДнейСПлатежами,
    SUM(CASE WHEN Сумма = 0 THEN 1 ELSE 0 END) AS ДнейБезПлатежей
FROM #Result1

UNION ALL

SELECT 
    'Пример 2' AS Пример,
    'ClientId=2, 2022-01-04 to 2022-01-11' AS Параметры,
    COUNT(*) AS ВсегоДней,
    SUM(Сумма) AS ОбщаяСумма,
    SUM(CASE WHEN Сумма > 0 THEN 1 ELSE 0 END) AS ДнейСПлатежами,
    SUM(CASE WHEN Сумма = 0 THEN 1 ELSE 0 END) AS ДнейБезПлатежей
FROM #Result2;
GO

-- =============================================
-- Детальная статистика по результатам
-- =============================================

PRINT '';
PRINT 'Детальная статистика:';
PRINT '----------------------------------------';

-- Статистика для примера 1
SELECT 
    'Пример 1' AS Пример,
    MIN(Dt) AS ПерваяДата,
    MAX(Dt) AS ПоследняяДата,
    COUNT(*) AS ВсегоДней,
    MIN(Сумма) AS МинСумма,
    MAX(Сумма) AS МаксСумма,
    AVG(Сумма) AS СредняяСумма,
    SUM(Сумма) AS ОбщаяСумма
FROM #Result1

UNION ALL

SELECT 
    'Пример 2' AS Пример,
    MIN(Dt) AS ПерваяДата,
    MAX(Dt) AS ПоследняяДата,
    COUNT(*) AS ВсегоДней,
    MIN(Сумма) AS МинСумма,
    MAX(Сумма) AS МаксСумма,
    AVG(Сумма) AS СредняяСумма,
    SUM(Сумма) AS ОбщаяСумма
FROM #Result2;
GO

-- Очистка временных таблиц
IF OBJECT_ID('tempdb..#Result1', 'U') IS NOT NULL
    DROP TABLE #Result1;

IF OBJECT_ID('tempdb..#Result2', 'U') IS NOT NULL
    DROP TABLE #Result2;
GO

PRINT '';
PRINT '========================================';
PRINT 'Вывод результатов завершен.';
PRINT '========================================';
GO

