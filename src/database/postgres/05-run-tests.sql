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
-- Run tests for GetDailyPayments function
-- =============================================

DO $$
BEGIN
    RAISE NOTICE '========================================';
    RAISE NOTICE 'Testing function GetDailyPayments';
    RAISE NOTICE '========================================';
    RAISE NOTICE '';
END $$;

-- Test 1: Example from assignment (ClientId=1, 2022-01-02 to 2022-01-07)
DO $$
BEGIN
    RAISE NOTICE 'Test 1: ClientId = 1, period 2022-01-02 to 2022-01-07';
    RAISE NOTICE 'Expected:';
    RAISE NOTICE '  2022-01-02 | 0';
    RAISE NOTICE '  2022-01-03 | 100';
    RAISE NOTICE '  2022-01-04 | 0';
    RAISE NOTICE '  2022-01-05 | 450';
    RAISE NOTICE '  2022-01-06 | 0';
    RAISE NOTICE '  2022-01-07 | 50';
    RAISE NOTICE 'Actual result:';
END $$;

SELECT * FROM public.getdailypayments(1, '2022-01-02'::date, '2022-01-07'::date) ORDER BY dt;

DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE 'Test 2: ClientId = 2, period 2022-01-04 to 2022-01-11';
    RAISE NOTICE 'Expected:';
    RAISE NOTICE '  2022-01-04 | 0';
    RAISE NOTICE '  2022-01-05 | 278';
    RAISE NOTICE '  2022-01-06 | 0';
    RAISE NOTICE '  2022-01-07 | 0';
    RAISE NOTICE '  2022-01-08 | 0';
    RAISE NOTICE '  2022-01-09 | 0';
    RAISE NOTICE '  2022-01-10 | 300';
    RAISE NOTICE '  2022-01-11 | 0';
    RAISE NOTICE 'Actual result:';
END $$;

SELECT * FROM public.getdailypayments(2, '2022-01-04'::date, '2022-01-11'::date) ORDER BY dt;

DO $$
BEGIN
    RAISE NOTICE '';
    RAISE NOTICE '========================================';
    RAISE NOTICE 'All tests completed.';
    RAISE NOTICE '========================================';
END $$;

