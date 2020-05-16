using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.Xunit2;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using Xunit;

namespace Pagination.Tests
{
    public class PageFactoryTests
    {
        PageFactory CreateSut()
        => new PageFactory();
        [Theory, AutoData]
        public void Paginate_WithNullRequest_ThrowsArgumentException(IEnumerable<int> dummy)
        {
            var sut = CreateSut();
            Action act = () => sut.Paginate(dummy.AsQueryable(), null);
            act.Should().Throw<ArgumentException>();
        }
        [Theory, AutoData]
        public void Paginate_WithNullItems_ReturnEmpty(PageRequest request)
        {
            var sut = CreateSut();
            var result = sut.Paginate<int>(null, request);
            result.ItemsInThePage.Should().BeEmpty().And.NotBeNull();
        }



        [Property]//Mathematical test:
        public void Paginate_WhenItemsAreIncludedInRequestedPage_ReturnsPagedItems(
            PositiveInt pageNumber, PositiveInt pageSize, NonEmptyArray<int> randomArray)
        {
            //TODO: move this to the arbitrary generators:
            var allItems = randomArray.Item.Select((item, i) => (Item: item, Index: i + 1))
                                        .AsQueryable();


            var request = new PageRequest
            {
                PageNumber = (uint)pageNumber.Item,
                PageSize = (uint)pageSize.Item
            };
            var sut = CreateSut();
            var result = sut.Paginate<(int Item, int Index)>(allItems, request);
            if (result.ItemsInThePage.Any())//TODO: remove this if statement by identifying the corresponding equivalent class arbitraries
            {
                var expected = (byte)((result.ItemsInThePage.First().Index / result.Counter.Size) + 1);
                result.Counter.Number.Should().Be(expected);
            }
        }

        //--

        [Theory]
        [InlineAutoData(10, 10, 1)]
        [InlineAutoData(10, 100, 10)]
        [InlineAutoData(10, 101, 11)]
        [InlineAutoData(10, 109, 11)]
        [InlineAutoData(10, 110, 11)]
        [InlineAutoData(10, 111, 12)]
        public void TotalPageCount_Allways_ReturnsTotalPageCount(
            byte pageSize, int totalRecordCount, uint expectedTotalPageCounts)
        {
            PageRequest request = new PageRequest
            {
                PageNumber = 1,
                PageSize = pageSize
            };
            var items = new Fixture().CreateMany<int>(totalRecordCount);
            var sut = CreateSut();
            var result = sut.Paginate(items.AsQueryable(), request);

            result.Counter.TotalPageCount.Should().Be(expectedTotalPageCounts);
        }

        [Theory, AutoData]
        public void Paginate_WithSizeLessThan5_SetsSizeTo5(PageRequest request, int[] dummy)
        {
            var sut = CreateSut();
            request.PageSize = 0;
            var result = sut.Paginate(dummy.AsQueryable(), request);
            result.Counter.Size.Should().Be(5);
        }

        [Theory, AutoData]
        public void Paginate_ByDefault_ReturnsPageSize(PageRequest request, int[] dummy)
        {
            var sut = CreateSut();
            var result = sut.Paginate(dummy.AsQueryable(), request);
            result.Counter.Size.Should().Be((byte)request.PageSize);
        }

        [Theory]
        [InlineData(2, 1, 1)]
        [InlineData(3, 10, 1)]
        public void Paginate_WithPageNumberBiggerThanTotalPageCount_ReturnsTotalPageCount(
            byte number, byte size, byte totalItemCount)
        {
            var request = new PageRequest
            {
                PageNumber = number,
                PageSize = size
            };
            var items = new Fixture().CreateMany<int>(totalItemCount).AsQueryable();
            var sut = CreateSut();

            var result = sut.Paginate(items, request);

            result.Counter.Number.Should().Be(result.Counter.TotalItemCount);
        }

        [Theory, AutoData]
        public void Paginate_WithPageNumberSmallerThan1_SetsPageNumberTo1(byte dummy)
        {
            var sut = CreateSut();
            var request = new PageRequest { PageNumber = 0, PageSize = dummy };
            var items = Enumerable.Range(1, 10).AsQueryable();
            var result = sut.Paginate(items, request);
            result.Counter.Number.Should().Be(1);
        }

        [Theory, AutoData]
        public void Paginate_WithPageNumber0_AndPageCountIs0_SetsPageNumberTo1(byte dummy)
        {
            var request = new PageRequest { PageNumber = 0, PageSize = dummy };
            var items = new int[0].AsQueryable();
            var sut = CreateSut();
            var result = sut.Paginate(items, request);
            result.Counter.Number.Should().Be(1);
        }

        [Theory]
        [InlineData(1, 10, 1)]
        [InlineData(10, 10, 100)]
        [InlineData(10, 10, 99)]
        [InlineData(10, 10, 98)]
        [InlineData(10, 10, 91)]
        [InlineData(10, 9, 90)]
        public void Paginate_WhenPageNumberIsTotalPageCount_HasNextIsFalse(
            byte number, byte size, byte totalItemCount)
        {
            var sut = CreateSut();
            var items = new Fixture().CreateMany<int>(totalItemCount).AsQueryable();
            var request = new PageRequest { PageNumber = number, PageSize = size };
            var result = sut.Paginate(items, request);
            result.Counter.HasNext.Should().BeFalse();
        }

        [Theory]
        [InlineData(1, 9, 20)]
        [InlineData(10, 9, 100)]
        [InlineData(10, 9, 99)]
        [InlineData(10, 9, 98)]
        [InlineData(10, 9, 91)]
        [InlineData(10, 8, 90)]
        public void Paginate_WhenPageNumberDoesNotEqualTotalPageCount_HasNextIsTrue(
            byte number, byte size, byte totalItemCount)
        {
            var sut = CreateSut();
            var items = new Fixture().CreateMany<int>(totalItemCount).AsQueryable();
            var request = new PageRequest { PageNumber = number, PageSize = size };
            var result = sut.Paginate(items, request);
            result.Counter.HasNext.Should().BeTrue();
        }

        [Theory, AutoData]
        public void Paginate_WhenPageNumberIs1_HasPreviousIsFalse(byte dummy, int[] dummyItems)
        {
            var request = new PageRequest { PageNumber = 1, PageSize = dummy };
            var sut = CreateSut();
            var result = sut.Paginate(dummyItems.AsQueryable(), request);
            result.Counter.HasPrevious.Should().BeFalse();
        }

        [Theory]
        [InlineData(2, 8, 4)]
        [InlineData(255, 1000, 1)]
        public void Paginate_WhenPageNumberIsBiggerThan1_AndThereAreMoreThan1Page_HasPreviousTrue(
            uint pageNumber, int totalItemCount, byte pageSize)
        {
            var items = new Fixture().CreateMany<int>(totalItemCount).AsQueryable();
            var request = new PageRequest { PageNumber = pageNumber, PageSize = pageSize };
            var sut = CreateSut();
            var result = sut.Paginate(items, request);
            result.Counter.HasPrevious.Should().BeTrue();
        }


        [Theory]
        [InlineAutoData(50)]
        [InlineAutoData(40)]
        [InlineAutoData(70)]
        public void Paginate_TotalItemCount_CanBeSmallerThanSize(byte pageSize, PageRequest request)
        {
            var totalItemCount = pageSize - 10;
            var items = new Fixture().CreateMany<int>(totalItemCount);
            request.PageSize = pageSize;
            var sut = CreateSut();
            Action act = () => sut.Paginate(items.AsQueryable(), request);
            act.Should().NotThrow<Exception>();
        }
    }
}