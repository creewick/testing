using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;

namespace HomeExercises
{
	public class NumberValidatorTests
	{
		// Разделил тесты на отдельные случаи. В каждом по несколько TestCase
		// Добавил дополнительные проверки
		[TestCase(-1, 2, true)]
		[TestCase(-1, 2, false)]
		[TestCase(1, 2, false)]
		[TestCase(1, 2, true)]
		public void ExceptionOnIncorrectArguments(int a, int b, bool flag)
		{
			Action createValidator = () => new NumberValidator(a, b, flag);
			createValidator.ShouldThrow<ArgumentException>();
		}

		[TestCase(1, 0, true)]
		public void NoExceptionOnCorrectArguments(int a, int b, bool flag)
		{
			Action createValidator = () => new NumberValidator(a, b, flag);
			createValidator.ShouldNotThrow<ArgumentException>();
		}

		// У следующих методов одинаковое тело. Надеюсь, повторение кода в этом случае приемлимо
		[TestCase(17, 2, true, "1.23", ExpectedResult = true)]
		[TestCase(3, 2, true, "1.23", ExpectedResult = true)]
		[TestCase(2, 1, true, "1.23", ExpectedResult = false)]
		public bool PrecisionInGeneral(int a, int b, bool flag, string number)
		{
			return new NumberValidator(a, b, flag).IsValidNumber(number);
		}

		[TestCase(3, 1, true, "+1.2", ExpectedResult = true)]
		[TestCase(2, 1, true, "+1.2", ExpectedResult = false)]
		[TestCase(3, 1, false, "-1.2", ExpectedResult = true)]
		[TestCase(2, 1, false, "-1.2", ExpectedResult = false)]
		public bool PrecisionWithSign(int a, int b, bool flag, string number)
		{
			return new NumberValidator(a, b, flag).IsValidNumber(number);
		}

		[TestCase(10, 2, true, "1.2", ExpectedResult = true)]
		[TestCase(10, 2, true, "1.23", ExpectedResult = true)]
		[TestCase(10, 2, true, "1.234", ExpectedResult = false)]
		public bool ScaleInGeneral(int a, int b, bool flag, string number)
		{
			return new NumberValidator(a, b, flag).IsValidNumber(number);
		}

		[TestCase(10, 2, true, "+1.23", ExpectedResult = true)]
		[TestCase(10, 2, false, "-1.23", ExpectedResult = true)]
		public bool ScaleWithSign(int a, int b, bool flag, string number)
		{
			return new NumberValidator(a, b, flag).IsValidNumber(number);
		}

		[TestCase(10, 2, true, "1.2", ExpectedResult = true)]
		[TestCase(10, 2, true, "-1.2", ExpectedResult = false)]
		[TestCase(10, 2, false, "1.2", ExpectedResult = true)]
		[TestCase(10, 2, false, "-1.2", ExpectedResult = true)]
		public bool OnlyPositive(int a, int b, bool flag, string number)
		{
			return new NumberValidator(a, b, flag).IsValidNumber(number);
		}

		[TestCase(10, 2, true, null, ExpectedResult = false)]
		[TestCase(10, 2, true, "", ExpectedResult = false)]
		public bool NullOrEmpty(int a, int b, bool flag, string number)
		{
			return new NumberValidator(a, b, flag).IsValidNumber(number);
		}

		[TestCase(10, 2, true, "a.bc", ExpectedResult = false)]
		[TestCase(10, 2, false, "-zx", ExpectedResult = false)]
		public bool IncorrectString(int a, int b, bool flag, string number)
		{
			return new NumberValidator(a, b, flag).IsValidNumber(number);
		}
	}

	public class NumberValidator
	{
		private readonly Regex numberRegex;
		private readonly bool onlyPositive;
		private readonly int precision;
		private readonly int scale;

		public NumberValidator(int precision, int scale = 0, bool onlyPositive = false)
		{
			this.precision = precision;
			this.scale = scale;
			this.onlyPositive = onlyPositive;
			if (precision <= 0)
				throw new ArgumentException("precision must be a positive number");
			if (scale < 0 || scale >= precision)
				throw new ArgumentException("precision must be a non-negative number less or equal than precision");
			numberRegex = new Regex(@"^([+-]?)(\d+)([.,](\d+))?$", RegexOptions.IgnoreCase);
		}

		public bool IsValidNumber(string value)
		{
			// Проверяем соответствие входного значения формату N(m,k), в соответствии с правилом, 
			// описанным в Формате описи документов, направляемых в налоговый орган в электронном виде по телекоммуникационным каналам связи:
			// Формат числового значения указывается в виде N(m.к), где m – максимальное количество знаков в числе, включая знак (для отрицательного числа), 
			// целую и дробную часть числа без разделяющей десятичной точки, k – максимальное число знаков дробной части числа. 
			// Если число знаков дробной части числа равно 0 (т.е. число целое), то формат числового значения имеет вид N(m).

			if (string.IsNullOrEmpty(value))
				return false;

			var match = numberRegex.Match(value);
			if (!match.Success)
				return false;

			// Знак и целая часть
			var intPart = match.Groups[1].Value.Length + match.Groups[2].Value.Length;
			// Дробная часть
			var fracPart = match.Groups[4].Value.Length;

			if (intPart + fracPart > precision || fracPart > scale)
				return false;

			if (onlyPositive && match.Groups[1].Value == "-")
				return false;
			return true;
		}
	}
}