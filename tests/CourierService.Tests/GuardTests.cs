using System;
using Xunit;
using CourierService.Application.Common;
using System.Collections.Generic;

namespace CourierService.Tests
{
    public class GuardTests
    {
        [Fact]
        public void NotNull_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Guard.NotNull(null, "value"));
        }

        [Fact]
        public void NotNullOrEmpty_Null_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => Guard.NotNullOrEmpty<string>(null, "list"));
        }

        [Fact]
        public void NotNullOrEmpty_Empty_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Guard.NotNullOrEmpty(new List<int>(), "list"));
        }

        [Fact]
        public void NonNegative_Negative_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Guard.NonNegative(-1m, "value"));
        }

        [Fact]
        public void GreaterThanZero_Zero_ThrowsArgumentException()
        {
            Assert.Throws<ArgumentException>(() => Guard.GreaterThanZero(0m, "value"));
        }
    }
}
