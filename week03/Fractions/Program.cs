using System;

class Fraction
{
    private int _numerator;
    private int _denominator;

    // Constructors
    public Fraction()
    {
        _numerator = 1;
        _denominator = 1;
    }

    public Fraction(int numerator)
    {
        _numerator = numerator;
        _denominator = 1;
    }

    public Fraction(int numerator, int denominator)
    {
        if (denominator == 0)
        {
            throw new ArgumentException("Denominator cannot be zero.");
        }
        _numerator = numerator;
        _denominator = denominator;
    }

    // Getters and Setters
    public int GetNumerator() => _numerator;
    public void SetNumerator(int numerator) => _numerator = numerator;

    public int GetDenominator() => _denominator;
    public void SetDenominator(int denominator)
    {
        if (denominator == 0)
        {
            throw new ArgumentException("Denominator cannot be zero.");
        }
        _denominator = denominator;
    }

    // Methods
    public string GetFractionString() => $"{_numerator}/{_denominator}";
    public double GetDecimalValue() => (double)_numerator / _denominator;
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello World! This is the Fractions Project.");
    }
}