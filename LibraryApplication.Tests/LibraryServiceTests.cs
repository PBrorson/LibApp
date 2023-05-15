using Library.Database.Models;
using Library.Services.Interface.services;
using Moq;

namespace LibraryApplication.Tests
{
    using Library.Database.Models;
    using Library.Services.Services;
    using Moq;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Xunit;

    namespace Tests
    {
        public class LibraryServiceTests
        {
            [Fact]
            public async Task GetLibraryItemsAsync_ReturnsAllItems()
            {
             
                var mockLibraryService = new Mock<ILibraryService>();
                mockLibraryService.Setup(service => service.GetLibraryItemsAsync())
                    .ReturnsAsync(GetSampleLibraryItems());
                var service = mockLibraryService.Object;
                var items = await service.GetLibraryItemsAsync();
                           
                Assert.Equal(2, items.Count());
            }

            [Fact]
            public async Task GetLibraryItemByIdAsync_ReturnsCorrectItem()
            {
               
                var mockLibraryService = new Mock<ILibraryService>();
                mockLibraryService.Setup(service => service.GetLibraryItemByIdAsync(It.IsAny<int>()))
                    .ReturnsAsync((int id) => GetSampleLibraryItems().FirstOrDefault(item => item.Id == id));

                var service = mockLibraryService.Object;
                var item = await service.GetLibraryItemByIdAsync(1);
                              
                Assert.NotNull(item);
                Assert.Equal(1, item.Id);
                Assert.Equal("Test Item1", item.Title);
            }

            [Fact]
            public async Task CreateLibraryItemAsync_AddsNewItem()
            {
                var newItem = new LibraryItem { Title = "New TestItem" };
                var mockLibraryService = new Mock<ILibraryService>();
                mockLibraryService.Setup(service => service.CreateLibraryItemAsync(It.IsAny<LibraryItem>()))
                    .Callback<LibraryItem>(item => GetSampleLibraryItems().ToList().Add(item))
                    .Returns(Task.CompletedTask);

                var service = mockLibraryService.Object;
                await service.CreateLibraryItemAsync(newItem);

                var items = GetSampleLibraryItems();
                Assert.Contains(items, item => item.Title == "New Test Item");
            }
            private IEnumerable<LibraryItem> GetSampleLibraryItems()
            {
                return new List<LibraryItem>
            {
                new LibraryItem { Id = 1, Title = "Test Item1" },
                new LibraryItem { Id = 2, Title = "Test Item2" }
            };
            }
        }
    }

}