using FsCheck;
using System;
using System.Collections.Generic;
using AutoFixture.Xunit2;
using Xunit;
using FluentAssertions;
using System.Linq;
using FsCheck.Xunit;

namespace Pagination.Tests
{
    public class PagedViewModelTests
    {
        [Theory, AutoData]
        public void Counter_WhenNull_ThrowsArgumentException(IEnumerable<int> dummy)
        {
            Action act = () => new PagedViewModel<int>(null, dummy.AsQueryable());
            act.Should().Throw<ArgumentException>();
        }
        [Theory, AutoData]
        public void Counter_ByDefault_ReturnsCounter(IEnumerable<int> dummy)
        {
            var counter = BuildPageCounter.AValidOne().Please();
            var sut = new PagedViewModel<int>(counter, dummy.AsQueryable());
            sut.Counter.Should().Be(counter);
        }

        [Theory, AutoData]
        public void Items_WhenNull_ReturnEmptyAndNotNull()
        {
            var dummy = BuildPageCounter.AValidOne().Please();
            var sut = new PagedViewModel<int>(dummy, null);
            sut.ItemsInThePage.Should().BeEmpty().And.NotBeNull();
        }

        [Property]//Mathematical test:
        public void Items_WhenSomeItemsAreIncludedInRequestedPage_ReturnsPagedItems(
            PositiveInt pageNumber, PositiveInt pageSize, NonEmptyArray<int> randomArray)
        {
            //TODO: move this to the arbitrary generators:
            var allItems = randomArray.Item.Select((item, i) => (Item: item, Index: i + 1))
                                        .AsQueryable();

            var pageCounter = BuildPageCounter.AValidOne()
                                              .WithPageNumber((uint)pageNumber.Item)
                                              .WithPageSize((byte)pageSize.Item)
                                              .Please();

            var sut = new PagedViewModel<(int Item, int Index)>(pageCounter, allItems);
            if (sut.ItemsInThePage.Any())//TODO: remove this if statement by identifying the corresponding equivalent class arbitraries
            {
                var expected = (byte)((sut.ItemsInThePage.First().Index / pageCounter.Size) + 1);
                pageCounter.Number.Should().Be(expected);
            }
        }
    }
}