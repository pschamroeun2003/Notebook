using System;
using System.ComponentModel.DataAnnotations;
namespace NoteTakingApp.DTOs
{
    public class ApiResponse<T>
    {
        public string Status { get; set; }
        public T Data { get; set; }
    }
}