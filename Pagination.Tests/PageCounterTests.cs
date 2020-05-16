using FsCheck;
using AutoFixture.Xunit2;
using Xunit;
using FluentAssertions;
using FsCheck.Xunit;
using System;

namespace Pagination.Tests
{
    public class PageCounterTests
    {
        [Theory]
        [InlineData(10, 10, 1)]
        [InlineData(10, 100, 10)]
        [InlineData(10, 101, 11)]
        [InlineData(10, 109, 11)]
        [InlineData(10, 110, 11)]
        [InlineData(10, 111, 12)]
        public void TotalPageCount_Allways_ReturnsTotalPageCount(
            byte pageSize, uint totalRecordCount, uint expectedTotalPageCount)
        {
            var sut = BuildPageCounter.AValidOne()
                                      .WithPageSize(pageSize)
                                      .WithTotalItemCount(totalRecordCount)
                                      .Please();

            sut.TotalPageCount.Should().Be(expectedTotalPageCount);

            //TODO: refactor to property-based test:
            (sut.TotalPageCount * pageSize).Should().BeCloseTo(sut.TotalItemCount, 10);
        }

        [Fact]
        public void Size_When0_Returns5()
        {
            var sut = BuildPageCounter.AValidOne().WithPageSize(0).Please();
            sut.Size.Should().Be(5);
        }

        [Theory]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(255)]
        public void Size_ByDefault_ReturnsPageSize(byte expectedSize)
        {
            var result = BuildPageCounter.AValidOne().WithPageSize(expectedSize).Please();
            result.Size.Should().Be(expectedSize);
        }

        [Theory]
        [InlineData(2, 1, 1)]
        [InlineData(3, 10, 1)]
        public void PageNumber_WhenBiggerThanTotalPageCount_ReturnsTotalPageCount(
            byte number, byte size, byte totalItemCount)
        {
            var sut = BuildPageCounter.AValidOne()
                                      .WithPageNumber(number)
                                      .WithPageSize(size)
                                      .WithTotalItemCount(totalItemCount)
                                      .Please();
            sut.Number.Should().Be(sut.TotalPageCount);
        }

        [Fact]
        public void PageNumber_WhenSmallerThan1_Returns1()
        {
            var sut = BuildPageCounter.AValidOne()
                                      .WithPageNumber(0)
                                      .Please();
            sut.Number.Should().Be(1);
        }

        [Fact]
        public void PageNumber_When0_AndPageCountIs0_Returns1()
        {
            var sut = BuildPageCounter.AValidOne()
                                      .WithPageNumber(0)
                                      .WithTotalItemCount(0)
                                      .Please();
            sut.Number.Should().Be(1);
        }

        [Theory]
        [InlineData(1, 10, 1)]
        [InlineData(10, 10, 100)]
        [InlineData(10, 10, 99)]
        [InlineData(10, 10, 98)]
        [InlineData(10, 10, 91)]
        [InlineData(10, 9, 90)]
        public void HasNext_WhenPageNumberIsTotalPageCount_RetunsFalse(
            byte number, byte size, byte totalItemCount)
        {
            var sut = BuildPageCounter.AValidOne()
                                      .WithPageNumber(number)
                                      .WithPageSize(size)
                                      .WithTotalItemCount(totalItemCount)
                                      .Please();
            sut.HasNext.Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 9, 20)]
        [InlineData(10, 9, 100)]
        [InlineData(10, 9, 99)]
        [InlineData(10, 9, 98)]
        [InlineData(10, 9, 91)]
        [InlineData(10, 8, 90)]
        public void HasNext_WhenPageNumberDoesNotEqualTotalPageCount_ReturnsTrue(
            byte number, byte size, byte totalItemCount)
        {
            var sut = BuildPageCounter.AValidOne()
                                      .WithPageNumber(number)
                                      .WithPageSize(size)
                                      .WithTotalItemCount(totalItemCount)
                                      .Please();
            sut.HasNext.Should().BeTrue();
        }

        [Fact]
        public void HasPrevious_WhenPageNumberIs1_ReturnsFalse()
        {
            var sut = BuildPageCounter.AValidOne().WithPageNumber(1).Please();
            sut.HasPrevious.Should().BeFalse();
        }

        [Theory]
        [InlineData(2)]
        [InlineData(255)]
        public void HasPrevious_WhenPageNumberIsBiggerThan1_ReturnsTrue(byte pageNumber)
        {
            var sut = BuildPageCounter.AValidOne().WithPageNumber(pageNumber).Please();
            sut.HasPrevious.Should().BeTrue();
        }


        [Theory]
        [InlineData(50)]
        [InlineData(40)]
        [InlineData(70)]
        public void TotalItemCount_CanBeSmallerThanSize(byte pageSize)
        {
            var totalItemCount = (uint)pageSize - 10;
            Action act = () => BuildPageCounter.AValidOne()
                                      .WithPageSize(pageSize)
                                      .WithTotalItemCount(totalItemCount)
                                      .Please();
            act.Should().NotThrow<Exception>();
        }


    }
}