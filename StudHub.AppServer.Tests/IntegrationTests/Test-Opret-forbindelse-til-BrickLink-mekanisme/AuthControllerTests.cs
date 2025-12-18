// https://gemini.google.com/ 18-12-2025

using System.Net;
using System.Net.Http.Json;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Studhub.Grpc.Data;
using StudHub.SharedDTO.StoreCredentials;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;

namespace StudHub.AppServer.Tests.IntegrationTests.
    Test_Opret_forbindelse_til_BrickLink_mekanisme;

public class
    AuthControllerTests : IClassFixture<WebApplicationFactory<Program>>,
    IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly WireMockServer _wireMockServer;

    public AuthControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _wireMockServer = WireMockServer.Start();
    }

[Fact]
public async Task SetBrickLinkCredentials_FullFlow_ReturnsOk()
{
    // --- Arrange ---
    
    // POINT 2.1.1: Setup WireMock to simulate External BrickLink API responding to the test call
    _wireMockServer
        .Given(Request.Create().WithPath("/api/store/v1/items/PART/3001").UsingGet())
        .RespondWith(Response.Create().WithStatusCode(200)
            .WithBody("{\"meta\":{\"code\":200}}"));

    // POINT 3: Setup Mock for gRPC Data Server call
    var grpcMock = new Mock<StudService.StudServiceClient>();
    grpcMock.Setup(x =>
            x.SetBrickLinkAuthByIdAsync(It.IsAny<SetBrickLinkAuthByIdRequest>(), null, null, default))
        .Returns(new AsyncUnaryCall<SetBrickLinkAuthByIdResponse>(
            Task.FromResult(new SetBrickLinkAuthByIdResponse { IsSuccess = true }),
            Task.FromResult(new Metadata()), () => Status.DefaultSuccess, () => new Metadata(), () => { }));

    var client = _factory.WithWebHostBuilder(builder =>
    {
        builder.ConfigureAppConfiguration((context, configBuilder) =>
        {
            configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ApiUrls:BrickLinkTest"] = $"{_wireMockServer.Url}/api/store/v1/items/PART/3001"
            });
        });
        builder.ConfigureServices(services => { services.AddSingleton(grpcMock.Object); });
    }).CreateClient();

    // --- Act ---
    
    // POINT 1.1: Client sends Put request to App Server
    var request = new BrickLinkCredentialsRequestDTO
    {
        StudUserId = 1, ConsumerKey = "key", ConsumerSecret = "sec",
        TokenValue = "val", TokenSecret = "s-sec"
    };
    var response = await client.PutAsJsonAsync("auth/bricklink-connect", request);

    // --- Assert ---
    
    // Verify POINT 4 & Final Response: Check if the full flow returns success
    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    var result = await response.Content.ReadFromJsonAsync<BrickLinkCredentialsResponseDTO>();
    Assert.NotNull(result);
    Assert.True(result.IsSucces);

    // Verify POINT 3: Ensure gRPC call was actually triggered once POINT 2.1.1 succeeded
    grpcMock.Verify(x => x.SetBrickLinkAuthByIdAsync(It.IsAny<SetBrickLinkAuthByIdRequest>(), null, null, default), Times.Once);
}

[Fact]
public async Task SetBrickLinkCredentials_WrongCredentials_ReturnsUnauthorized()
{
    // --- Arrange ---
    
    // POINT 2.1.1: Setup WireMock to simulate a FAILED External BrickLink API call (401 Unauthorized)
    _wireMockServer
        .Given(Request.Create().WithPath("/api/store/v1/items/PART/3001").UsingGet())
        .RespondWith(Response.Create().WithStatusCode(401));

    var grpcMock = new Mock<StudService.StudServiceClient>();

    var client = _factory.WithWebHostBuilder(builder => {
        /* ... same setup as above ... */
        builder.ConfigureServices(services => { services.AddSingleton(grpcMock.Object); });
    }).CreateClient();

    // --- Act ---
    var request = new BrickLinkCredentialsRequestDTO { /* ... dummy data ... */ };
    var response = await client.PutAsJsonAsync("auth/bricklink-connect", request);

    // --- Assert ---
    Assert.False(response.IsSuccessStatusCode); 
    
    // RAINY DAY CHECK: Verify that POINT 3 (gRPC call) was NEVER called because POINT 2.1.1 failed
    grpcMock.Verify(x => x.SetBrickLinkAuthByIdAsync(It.IsAny<SetBrickLinkAuthByIdRequest>(), null, null, default), Times.Never);
}

    public void Dispose()
    {
        _wireMockServer.Stop();
        _wireMockServer.Dispose();
    }
}