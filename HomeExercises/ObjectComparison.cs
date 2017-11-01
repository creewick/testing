using FluentAssertions;
using FluentAssertions.Common;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();

			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));

		    var expectedPerson = expectedTsar;
		    var actualPerson = actualTsar;
            
            // Мы не будем проверять ID и не будем проверять всех предков
		    expectedPerson.ShouldBeEquivalentTo(actualPerson, options => options
		        .Excluding(o => o.Id)
		        .Excluding(o => o.Parent.Id)
                .Excluding(o => o.Parent.Parent)
		    );
		}

		[Test]
		[Description("Альтернативное решение. Какие у него недостатки?")]
        // Проблема: Синхронизация между кодом и тестом. При дополнении поля в класс, 
        //   проверку для этого поля в тесты придется добавлять вручную.
        //   Если забыть и окажется, что в новом коде ошибка, то тест будет зеленый
        // Проблема: Рекурсивный вызов. Может возникнуть переполнение стека вызовов.
        //   При этом окажется, что код верный, а тест падает
        public void CheckCurrentTsar_WithCustomEquality()
		{
			var actualTsar = TsarRegistry.GetCurrentTsar();
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
			new Person("Vasili III of Russia", 28, 170, 60, null));
            
		    AreEqual(actualTsar, expectedTsar).Should().BeTrue();
		}

		private bool AreEqual(Person actual, Person expected)
		{
			if (actual == expected) return true;
			if (actual == null || expected == null) return false;
			return
			actual.Name == expected.Name
			&& actual.Age == expected.Age
			&& actual.Height == expected.Height
			&& actual.Weight == expected.Weight
			&& AreEqual(actual.Parent, expected.Parent);
		}
	}

	public class TsarRegistry
	{
		public static Person GetCurrentTsar()
		{
			return new Person(
				"Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null));
		}
	}

	public class Person
	{
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public Person Parent;
		public int Id;

		public Person(string name, int age, int height, int weight, Person parent)
		{
			Id = IdCounter++;
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}
}
