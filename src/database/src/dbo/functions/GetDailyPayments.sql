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
-- Табличная функция для получения поденных сумм платежей клиента
-- =============================================
-- Описание: Возвращает поденные суммы платежей для указанного клиента
--           в заданном интервале дат. Если за день не было платежей,
--           возвращает 0.
-- Параметры:
--   @ClientId - идентификатор клиента
--   @Sd       - начальная дата интервала (включительно)
--   @Ed       - конечная дата интервала (включительно)
-- Возвращает:
--   Dt        - дата
--   Сумма     - сумма платежей за день (0 если платежей не было)
-- =============================================

USE TaskForgeDB;
GO

-- Удаление функции, если она существует
IF OBJECT_ID('dbo.GetDailyPayments', 'TF') IS NOT NULL
BEGIN
    DROP FUNCTION dbo.GetDailyPayments;
END
GO

-- Создание функции
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
        -- Генерируем все даты в указанном интервале
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
    OPTION (MAXRECURSION 10000)  -- Ограничение для рекурсивного CTE (до ~27 лет)
);
GO

PRINT 'Function dbo.GetDailyPayments created successfully.';
GO

