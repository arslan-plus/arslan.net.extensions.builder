using System;
using Xunit;

namespace Arslan.Net.Extensions.Builder {
    public abstract class PersonBase {

        private int _birthYear;
        public PersonBase(string firstName, string lastName, int birthYear) {
            FirstName = firstName;
            LastName = lastName;
            _birthYear = birthYear;
        }

        public string FirstName { get; }
        public string LastName { get; }
        private int BirthYear { get => _birthYear; }

        public int Age() => DateTime.UtcNow.Year - _birthYear;
    }

    public class Person : PersonBase
    {
        public Person(string firstName, string lastName, int birthYear, string sex, Person father = null, Person mother = null) : base(firstName, lastName, birthYear) {
            Sex = sex;
            Father = father;
            Mother = mother;
        }
        public string Sex { get; }
        public Person Father { get; }
        public Person Mother { get; }

    }


    public class BuilderTests {
        [Fact]
        public void Set_Get_Property() {
            var father = new Person("Jonathan", "Doe", 1960, "Male");
            var mother = new Person("Julie", "Doe", 1960, "Female");
            var source = new Person("John", "Doe", 1980, "Male", father, mother);
            var target = source.Set(x => x.FirstName, "Joe").Build();

            Assert.Equal("John", source.FirstName);
            Assert.Equal("Joe", target.FirstName);
        }

        [Fact]
        public void Set_Sub_Property() {
            var father = new Person("Jonathan", "Doe", 1960, "Male");
            var mother = new Person("Julie", "Doe", 1960, "Female");
            var source = new Person("John", "Doe", 1980, "Male", father, mother);
            var target = source.Set(x => x.Father.FirstName, "Joe").Build();

            Assert.Equal("Jonathan", source.Father.FirstName);
            Assert.Equal("Joe", target.Father.FirstName);
        }

        [Fact]
        public void Set_Private_Field() {
            var source = new Person("John", "Doe", 1980, "Male");
            var target = source.Set("_birthYear", 1985).Build();

            Assert.Equal(source.Age()-5, target.Age());
        }

        [Fact]
        public void Implicit_Conversion() {
            var source = new Person("John", "Doe", 1980, "Male");
            Person target = source.Set("_birthYear", 1985);

            Assert.Equal(source.GetType().FullName, target.GetType().FullName) ;
        }
    }
}
