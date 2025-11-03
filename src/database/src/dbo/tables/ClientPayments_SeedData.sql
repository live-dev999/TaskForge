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
-- Тестовые данные для таблицы ClientPayments
-- =============================================
-- Описание: Вставляет тестовые данные для проверки функции GetDailyPayments
-- =============================================

USE TaskForgeDB;
GO

-- Очистка существующих данных (опционально)
-- TRUNCATE TABLE dbo.ClientPayments;
-- GO

-- Вставка тестовых данных
INSERT INTO dbo.ClientPayments (ClientId, Dt, Amount) VALUES
    -- Данные для ClientId = 1 (для примера 1)
    (1, '2022-01-03 10:00:00', 100.00),
    (1, '2022-01-05 14:30:00', 450.00),
    (1, '2022-01-07 09:15:00', 50.00),
    
    -- Данные для ClientId = 2 (для примера 2)
    (2, '2022-01-05 11:20:00', 278.00),
    (2, '2022-01-10 16:45:00', 300.00);
GO

PRINT 'Test data inserted into dbo.ClientPayments successfully.';
PRINT 'Total records: ' + CAST(@@ROWCOUNT AS varchar(10));
GO

-- Проверка вставленных данных
SELECT 
    ClientId,
    COUNT(*) AS PaymentCount,
    SUM(Amount) AS TotalAmount,
    MIN(Dt) AS FirstPayment,
    MAX(Dt) AS LastPayment
FROM dbo.ClientPayments
GROUP BY ClientId
ORDER BY ClientId;
GO

