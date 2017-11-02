using System;
using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Equivalency;
using NUnit.Framework;

namespace HomeExercises
{
	public class ObjectComparison
	{
		private Person actualTsar;

		[SetUp]
		public void SetUp()
		{
			actualTsar = TsarRegistry.GetCurrentTsar();
		}
		[Test]
		[Description("Проверка текущего царя")]
		[Category("ToRefactor")]
		public void CheckCurrentTsar()
		{
			//Это решение
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null, 1), 1);

		    expectedTsar.ShouldBeEquivalentTo(actualTsar, options => options
		        .Excluding(info => info.SelectedMemberInfo.Name == "Id" &&
								   info.SelectedMemberInfo.DeclaringType == typeof(Person))
		    );
		}

		[Test]
		public void CheckIf_DifferentJobId()
		{
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null, 1), 2);
			
			expectedTsar.ShouldBeEquivalentTo(actualTsar, options => options
				.Excluding(info => info.SelectedMemberInfo.Name == "Id" &&
				                   info.SelectedMemberInfo.DeclaringType == typeof(Person))
			);
		}

		[Test]
		public void CheckIf_DifferentParentJobId()
		{
			var expectedTsar = new Person("Ivan IV The Terrible", 54, 170, 70,
				new Person("Vasili III of Russia", 28, 170, 60, null, 2), 1);

			expectedTsar.ShouldBeEquivalentTo(actualTsar, options => options
				.Excluding(info => info.SelectedMemberInfo.Name == "Id" &&
				                   info.SelectedMemberInfo.DeclaringType == typeof(Person))
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
			new Person("Vasili III of Russia", 28, 170, 60, null, 1), 1);
            
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
				new Person("Vasili III of Russia", 28, 170, 60, null, 1), 1);
		}
	}

	public class Person
	{
		public static int IdCounter = 0;
		public int Age, Height, Weight;
		public string Name;
		public Person Parent;
		public int Id;
		public Job Job;

		public Person(string name, int age, int height, int weight, Person parent, int jobId)
		{
			Id = IdCounter++;
			Job = new Job {Id = jobId};
			Name = name;
			Age = age;
			Height = height;
			Weight = weight;
			Parent = parent;
		}
	}

	public class Job
	{
		public int Id;
	}
}
