using System;
using System.Collections.Generic;
using System.Text;

namespace Ewan.Application.DTOs.Common
{
    // شكل موحّد لكل الـ Responses عشان الفرونت يتعامل بنفس الطريقة مع أي Endpoint
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> Ok(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }
}
