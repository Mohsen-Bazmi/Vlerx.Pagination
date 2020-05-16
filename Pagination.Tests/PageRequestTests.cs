using System;
using Xunit;
using FluentAssertions;

namespace Pagination.Tests
{
    public class PageRequestTests
    {
        [Fact]
        public void SetPageSize_WithPageSizeMoreThan255_Sets255()
        {
            var sut = new PageRequest();
            sut.PageSize = 500u;
            sut.PageSize.Should().Be(255u);
        }
        [Fact]
        public void PageSize_WhenNotSet_Is10()
        {
            var sut = new PageRequest();
            sut.PageSize.Should().Be(10u);
        }
        [Fact]
        public void PageNumber_WhenNotSet_Is1()
        {
            var sut = new PageRequest();
            sut.PageNumber.Should().Be(1u);
        }
    }
}
