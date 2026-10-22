using System.Security.Cryptography;
using System.Text;
using HuellaApi.Models;

namespace HuellaApi.Services;

public class HashService
{
    public string Compute(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }

    // Este método es el que usa TraceController en línea 42
    public string ComputeFor(TraceEvent e)
    {
        if (e == null) return string.Empty;
        
        var input = $"{e.BatchId}|{e.Stage}|{e.Location}|{e.Timestamp:O}|{e.Data}|{e.PrevHash}";
        return Compute(input);
    }
}