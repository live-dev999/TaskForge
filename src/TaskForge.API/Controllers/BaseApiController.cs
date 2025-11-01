/*
 *   Copyright (c) 2025 Dzianis Prokharchyk
 *   All rights reserved.

 *   Permission is hereby granted, free of charge, to any person obtaining a copy
 *   of this software and associated documentation files (the "Software"), to deal
 *   in the Software without restriction, including without limitation the rights
 *   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 *   copies of the Software, and to permit persons to whom the Software is
 *   furnished to do so, subject to the following conditions:
 
 *   The above copyright notice and this permission notice shall be included in all
 *   copies or substantial portions of the Software.
 
 *   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 *   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 *   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 *   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 *   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 *   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 *   SOFTWARE.
 */

using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using TaskForge.Application.Core;

namespace TaskForge.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BaseApiController : ControllerBase
{
    private IMediator _mediator;
    protected IMediator Mediator =>
        _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    protected IActionResult HandleResult<T>(Result<T> result)
    {
        if (result == null)
            return StatusCode(500, "Internal server error: result is null");

        if (result.IsSuccess)
        {
            // For reference types, check for null
            if (result.Value == null)
                return NotFound();

            // For numeric value types (int, long, decimal, etc.), treat default (0) as NotFound
            // But allow Guid.Empty, empty strings, empty collections, enums, Unit as valid values
            var type = typeof(T);
            if (type.IsValueType && !type.IsEnum)
            {
                // Exclude Guid, DateTime, TimeSpan, bool, char, Unit (MediatR), and other non-numeric types
                if (type != typeof(Guid) && 
                    type != typeof(DateTime) && 
                    type != typeof(DateTimeOffset) &&
                    type != typeof(TimeSpan) &&
                    type != typeof(bool) &&
                    type != typeof(char) &&
                    !(type.Name == "Unit" && type.Namespace == "MediatR")) // Unit from MediatR
                {
                    // Check if value equals default (0 for numbers)
                    if (Equals(result.Value, default(T)))
                        return NotFound();
                }
            }
            
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }
}
