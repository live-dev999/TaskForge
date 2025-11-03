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
-- Verify results - compare actual vs expected
-- =============================================

DO $$
BEGIN
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Verifying results';
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
END $$;

-- Test 1: Compare actual vs expected results
DO $$
BEGIN
    RAISE NOTICE 'Test 1 Comparison (ClientId=1, 2022-01-02 to 2022-01-07):';
    RAISE NOTICE '';
END $$;

WITH expected AS (
    SELECT dt::date, amount::numeric(18, 2) AS expected_amount
    FROM (VALUES
        ('2022-01-02'::date, 0::numeric(18, 2)),
        ('2022-01-03'::date, 100::numeric(18, 2)),
        ('2022-01-04'::date, 0::numeric(18, 2)),
        ('2022-01-05'::date, 450::numeric(18, 2)),
        ('2022-01-06'::date, 0::numeric(18, 2)),
        ('2022-01-07'::date, 50::numeric(18, 2))
    ) AS t(dt, amount)
),
actual AS (
    SELECT dt, amount AS actual_amount
    FROM public.getdailypayments(1, '2022-01-02', '2022-01-07')
)
SELECT 
    COALESCE(e.dt, a.dt) AS dt,
    e.expected_amount AS expected,
    a.actual_amount AS actual,
    CASE 
        WHEN e.expected_amount = a.actual_amount THEN '✓ PASS'
        WHEN e.dt IS NULL THEN '✗ MISSING IN EXPECTED'
        WHEN a.dt IS NULL THEN '✗ MISSING IN ACTUAL'
        ELSE '✗ FAIL'
    END AS status
    FROM expected e
FULL OUTER JOIN actual a ON e.dt = a.dt
ORDER BY COALESCE(e.dt, a.dt);

DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE 'Test 2 Comparison (ClientId=2, 2022-01-04 to 2022-01-11):';
    RAISE NOTICE '';
END $$;

WITH expected AS (
    SELECT dt::date, amount::numeric(18, 2) AS expected_amount
    FROM (VALUES
        ('2022-01-04'::date, 0::numeric(18, 2)),
        ('2022-01-05'::date, 278::numeric(18, 2)),
        ('2022-01-06'::date, 0::numeric(18, 2)),
        ('2022-01-07'::date, 0::numeric(18, 2)),
        ('2022-01-08'::date, 0::numeric(18, 2)),
        ('2022-01-09'::date, 0::numeric(18, 2)),
        ('2022-01-10'::date, 300::numeric(18, 2)),
        ('2022-01-11'::date, 0::numeric(18, 2))
    ) AS t(dt, amount)
),
actual AS (
    SELECT dt, amount AS actual_amount
    FROM public.getdailypayments(2, '2022-01-04', '2022-01-11')
)
SELECT 
    COALESCE(e.dt, a.dt) AS dt,
    e.expected_amount AS expected,
    a.actual_amount AS actual,
    CASE 
        WHEN e.expected_amount = a.actual_amount THEN '✓ PASS'
        WHEN e.dt IS NULL THEN '✗ MISSING IN EXPECTED'
        WHEN a.dt IS NULL THEN '✗ MISSING IN ACTUAL'
        ELSE '✗ FAIL'
    END AS status
    FROM expected e
FULL OUTER JOIN actual a ON e.dt = a.dt
ORDER BY COALESCE(e.dt, a.dt);

DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Verification completed.';
    RAISE NOTICE '========================================';
END $$;

