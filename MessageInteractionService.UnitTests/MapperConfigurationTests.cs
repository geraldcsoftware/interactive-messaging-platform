using AutoMapper;
using FluentAssertions;
using MessageInteractionService.Api.Mappings;
using MessageInteractionService.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace MessageInteractionService.UnitTests;

public class MapperConfigurationTests
{
    [Fact]
    public void MapperShouldBeConfiguredCorrectly()
    {
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMapper();
        serviceCollection.TryAddTransient<IDateTimeProvider, Clock>();
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact]
    public void MapIncomingMessage_ShouldMapCorrectly()
    {
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddMapper();
        serviceCollection.AddTransient<IDateTimeProvider>(_ => dateTimeProviderMock.Object);
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var mapper = serviceProvider.GetRequiredService<IMapper>();

        var expectedReceivedTime = DateTimeOffset.UtcNow;
        dateTimeProviderMock.Setup(d => d.UtcNow).Returns(expectedReceivedTime);

        var src = new MessageInteractionService.Api.Endpoints.Models.IncomingMessage
        {
            Body = "TEST", Sender = "TEST", Sent = null
        };

        var dest = mapper.Map<IncomingMessage>(src);

        dest.Body.Should().BeEquivalentTo(src.Body);
        dest.Sender.Should().BeEquivalentTo(src.Sender);
        dest.ReceivedTime.Should().Be(expectedReceivedTime);
    }
}