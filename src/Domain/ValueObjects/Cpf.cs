using System.Text.RegularExpressions;

namespace Domain.ValueObjects;

public readonly record struct Cpf
{
    public string Value { get; }

    public Cpf(string value)
    {
        var digits = OnlyDigits(value);
        if (!IsValid(digits))
            throw new ArgumentException("CPF inválido", nameof(value));
        Value = digits;
    }

    public static implicit operator string(Cpf cpf) => cpf.Value;
    public override string ToString() => Value;

    private static string OnlyDigits(string input) => Regex.Replace(input ?? string.Empty, "[^0-9]", "");

    // Validação clássica de CPF
    private static bool IsValid(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf) || cpf.Length != 11)
            return false;
        if (new string(cpf[0], cpf.Length) == cpf)
            return false;

        int[] mult1 = {10,9,8,7,6,5,4,3,2};
        int[] mult2 = {11,10,9,8,7,6,5,4,3,2};

        string tempCpf = cpf[..9];
        int sum = 0;
        for (int i = 0; i < 9; i++) sum += (tempCpf[i] - '0') * mult1[i];
        int remainder = sum % 11;
        int digit1 = remainder < 2 ? 0 : 11 - remainder;

        tempCpf += digit1.ToString();
        sum = 0;
        for (int i = 0; i < 10; i++) sum += (tempCpf[i] - '0') * mult2[i];
        remainder = sum % 11;
        int digit2 = remainder < 2 ? 0 : 11 - remainder;

        return cpf.EndsWith(digit1.ToString() + digit2);
    }
}

