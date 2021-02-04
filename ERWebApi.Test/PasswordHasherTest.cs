using ERService.Infrastructure.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Xunit.Abstractions;

namespace ERWebApi.Test
{
    public class PasswordHasherTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PasswordHasherTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void GenerateSaltedHash_NonEmptyPassword_ShouldReturnNonEmptySaltAndHash()
        {
            //  arrange
            var hasher = new PasswordHasher();
            var password = "123123";
            string hash;
            string salt;

            // act
            hasher.GenerateSaltedHash(password, out hash, out salt);

            // assert
            Assert.NotNull(hash);
            Assert.NotEmpty(hash);
            Assert.NotNull(salt);
            Assert.NotEmpty(salt);

            _testOutputHelper.WriteLine($"PW: {password} hash: {hash} salt: {salt}");
        }

        [Fact]
        public void VerifyPassword_NonEmptyParameters_ShouldReturnTrue()
        {
            //  arrange
            var hasher = new PasswordHasher();
            var password = "123123";
            string hash = "gBv+VtEf2cYHyegXS08ZqukGS9Ni7DfBkUvTwz8OZCg=";
            string salt = "hRdGCnBVAuqIoODRZ2/nlw==";

            // act
            var result = hasher.VerifyPassword(password, hash, salt);

            // assert
            Assert.True(result);
        }
    }
}
