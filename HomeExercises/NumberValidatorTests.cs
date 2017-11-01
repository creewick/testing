using System;
using System.Text.RegularExpressions;
using FluentAssertions;
using NUnit.Framework;
using NUnit.Framework.Internal.Filters;

namespace HomeExercises
{
    public class NumberValidatorTests
    {
        [TestCase(-1, 2, true, TestName = "NegativePercision")]
        [TestCase(0, 2, true, TestName = "ZeroPercision")]
        [TestCase(1, 2, false, TestName = "ScaleMoreThanPercision")]
        [TestCase(2, 2, false, TestName = "ScaleSameAsPercision")]
        [TestCase(1, -2, true, TestName = "ScaleNegative")]
        [TestCase(0, 0, true, TestName = "ZeroPercisionAndScale")]
        public void ThrowException_OnIncorrectArguments(int a, int b, bool flag)
        {
            Action createValidator = () => new NumberValidator(a, b, flag);
            createValidator.ShouldThrow<ArgumentException>();
        }

        [TestCase(1, 0, true, TestName = "PositiveScaleLessThanPositivePercision")]
        public void NoException_OnCorrectArguments(int a, int b, bool flag)
        {
            Action createValidator = () => new NumberValidator(a, b, flag);
            createValidator.ShouldNotThrow<ArgumentException>();
        }

        [TestCase(17, 2, true, "1.23", ExpectedResult = true,
            TestName = "True_EnoughPercision")]
        [TestCase(3, 2, true, "1.23", ExpectedResult = true,
            TestName = "True_EqualPercision")]
        [TestCase(2, 1, true, "1.23", ExpectedResult = false,
            TestName = "False_NotEnoughPercision")]
        [TestCase(17, 1, true, "+1.2", ExpectedResult = true,
            TestName = "True_PositveAllowed_WhenOnlyPositive")]
        [TestCase(17, 1, true, "-1.2", ExpectedResult = false,
            TestName = "False_NegativeDenied_WhenOnlyPositive")]
        [TestCase(17, 1, false, "+1.2", ExpectedResult = true,
            TestName = "True_PositiveAllowed_WhenNotOnlyPositive")]
        [TestCase(17, 1, false, "-1.2", ExpectedResult = true,
            TestName = "True_NegativeAllowed_WhenNotOnlyPositive")]
        [TestCase(3, 1, false, "-1.2", ExpectedResult = true,
            TestName = "True_DigitsWithSignEqualsPercision")]
        [TestCase(2, 1, false, "-1.2", ExpectedResult = false,
            TestName = "False_DigitsEqualsPercisionButWithSign")]
        [TestCase(10, 2, true, "1.2", ExpectedResult = true,
            TestName = "True_EnoughScale")]
        [TestCase(10, 2, true, "1.23", ExpectedResult = true,
            TestName = "True_EqualScale")]
        [TestCase(10, 2, true, "1.234", ExpectedResult = false,
            TestName = "False_NotEnoughScale")]
        [TestCase(10, 2, true, "+1.23", ExpectedResult = true,
            TestName = "True_EqualScaleWithSign")]
        [TestCase(10, 2, true, null, ExpectedResult = false,
            TestName = "False_NullString")]
        [TestCase(10, 2, true, "", ExpectedResult = false,
            TestName = "False_EmptyString")]
        [TestCase(10, 2, true, "    ", ExpectedResult = false,
            TestName = "False_WhiteSpaces")]
        [TestCase(10, 5, true, " 1.24 ", ExpectedResult = false,
            TestName = "False_NumberWithSpaces")]
        [TestCase(10, 5, true, "1,24", ExpectedResult = true,
            TestName = "True_CommaInsteadOfDot")]
        [TestCase(10, 5, true, "a.bc", ExpectedResult = false,
            TestName = "False_NotDigitsWithDot")]
        [TestCase(10, 5, true, "-ab", ExpectedResult = false,
            TestName = "False_NotDigitsWithSign")]
        public bool CheckAnswers(int a, int b, bool flag, string number)
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