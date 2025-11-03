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
-- Table-valued function for daily payment amounts
-- =============================================
-- Description: Returns daily payment amounts for specified client
--              within date range. Returns 0 for days without payments.
-- Parameters:
--   p_clientid - client identifier
--   p_startdate - start date of period (inclusive)
--   p_enddate - end date of period (inclusive)
-- Returns:
--   dt - date
--   amount - daily payment amount (0 if no payments)
-- =============================================

-- Drop function if exists
DROP FUNCTION IF EXISTS public.getdailypayments(bigint, date, date);

-- Create function
CREATE OR REPLACE FUNCTION public.getdailypayments(
    p_clientid bigint,
    p_startdate date,
    p_enddate date
)
RETURNS TABLE (
    dt date,
    amount numeric(18, 2)
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    WITH date_range AS (
        SELECT generate_series(
            p_startdate::timestamp,
            p_enddate::timestamp,
            '1 day'::interval
        )::date AS dt
    )
    SELECT 
        dr.dt,
        COALESCE(SUM(cp.amount), 0) AS amount
    FROM date_range dr
    LEFT JOIN public.clientpayments cp 
        ON cp.clientid = p_clientid 
        AND DATE(cp.dt) = dr.dt
    GROUP BY dr.dt;
END;
$$;

