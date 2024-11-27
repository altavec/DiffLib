﻿namespace DiffLib.Tests
{
    [TestFixture]
    public class ListExtensionsTests
    {
        [Test]
        public void Mutate_NullTarget_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ListExtensions.MutateToBeLike(null!, new int[0]));
        }

        [Test]
        public void Mutate_NullSource_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => ListExtensions.MutateToBeLike(new int[0], null!));
        }

        [Test]
        [TestCase("123456789", "123456789")]
        [TestCase("123456789", "12456789")]
        [TestCase("123456789", "1234x5a6789")]
        [TestCase("123456789", "1234556789")]
        [TestCase("123456789", "")]
        [TestCase("", "12456789")]
        [TestCase("123456789", "0")]
        [TestCase("123456789", "----------------------")]
        [TestCase("123456789", "987654321")]
        public void Mutate_TestCases(string target, string source)
        {
            var targetChars = target.ToCharArray().ToList();
            var sourceChars = source.ToCharArray();

            targetChars.MutateToBeLike(sourceChars);

            Assert.That(new string(targetChars.ToArray()), Is.EqualTo(source));
        }
    }
}
