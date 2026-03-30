using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuantityMeasurementModel.Dto;
using QuantityMeasurementRepository.Database;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace QuantityMeasurementApi.Tests
{
    /// <summary>
    /// UC17: Full integration tests using WebApplicationFactory.
    /// Starts the real ASP.NET Core pipeline with InMemory DB.
    /// Tests the complete stack: middleware → controller → service → repository.
    /// Spring equivalent: @SpringBootTest(webEnvironment=RANDOM_PORT) + TestRestTemplate.
    /// </summary>
    [TestClass]
    public class QuantityMeasurementApiIntegrationTests
    {
        private static WebApplicationFactory<Program> _factory = null!;
        private static HttpClient _client = null!;
        private static string _token = string.Empty;

        private static readonly JsonSerializerOptions _json = new() { PropertyNameCaseInsensitive = true };

        [ClassInitialize]
        public static async Task Init(TestContext _)
        {
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(host => host.ConfigureServices(services =>
                {
                    var desc = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<QuantityMeasurementDbContext>));
                    if (desc != null) services.Remove(desc);
                    services.AddDbContext<QuantityMeasurementDbContext>(opts =>
                        opts.UseInMemoryDatabase("TestDb_" + Guid.NewGuid()));
                }));
            _client = _factory.CreateClient();
            await SignupAndLogin();
        }

        [ClassCleanup]
        public static void Cleanup() { _client.Dispose(); _factory.Dispose(); }

        // ── HEALTH ────────────────────────────────────────────────────────

        [TestMethod]
        public async Task Health_Returns200() =>
            Assert.AreEqual(HttpStatusCode.OK, (await _client.GetAsync("/health")).StatusCode);

        // ── SWAGGER ───────────────────────────────────────────────────────

        [TestMethod]
        public async Task Swagger_Returns200()
        {
            var r = await _client.GetAsync("/swagger/v1/swagger.json");
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
            StringAssert.Contains(await r.Content.ReadAsStringAsync(), "Quantity Measurement API");
        }

        // ── AUTH ──────────────────────────────────────────────────────────

        [TestMethod]
        public async Task Signup_ValidRequest_Returns201WithToken()
        {
            var r = await Post("/api/v1/users/signup", new SignupRequestDTO
            {
                Username = "test_" + Guid.NewGuid().ToString()[..6],
                Email    = $"t_{Guid.NewGuid():N}@test.com",
                Password = "TestP@ss1"
            });
            Assert.AreEqual(HttpStatusCode.Created, r.StatusCode);
            var d = await Deserialize<ApiResponse<AuthResponseDTO>>(r);
            Assert.IsFalse(string.IsNullOrEmpty(d?.Data?.Token));
        }

        [TestMethod]
        public async Task Signup_DuplicateEmail_Returns409()
        {
            var email = $"dup_{Guid.NewGuid():N}@test.com";
            await Post("/api/v1/users/signup", new SignupRequestDTO { Username = "u1", Email = email, Password = "TestP@ss1" });
            var r = await Post("/api/v1/users/signup", new SignupRequestDTO { Username = "u2", Email = email, Password = "TestP@ss1" });
            Assert.AreEqual(HttpStatusCode.Conflict, r.StatusCode);
        }

        [TestMethod]
        public async Task Login_WrongPassword_Returns401()
        {
            var r = await Post("/api/v1/users/login", new LoginRequestDTO { Email = "x@x.com", Password = "Wrong1!" });
            Assert.AreEqual(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        // ── COMPARE ───────────────────────────────────────────────────────

        [TestMethod]
        public async Task Compare_1Feet_12Inches_ReturnsEqual()
        {
            Auth();
            var r = await Post("/api/v1/quantities/compare", new QuantityInputDTO
            {
                ThisQuantityDTO = new QuantityDTO(1.0, "FEET", "LENGTH"),
                ThatQuantityDTO = new QuantityDTO(12.0, "INCHES", "LENGTH")
            });
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
            var d = await Deserialize<ApiResponse<QuantityMeasurementDTO>>(r);
            Assert.AreEqual("EQUAL", d?.Data?.ResultUnit);
        }

        // ── CONVERT ───────────────────────────────────────────────────────

        [TestMethod]
        public async Task Convert_1Feet_Returns12Inches()
        {
            Auth();
            var r = await Post("/api/v1/quantities/convert", new ConvertRequestDTO
            {
                ThisQuantityDTO = new QuantityDTO(1.0, "FEET", "LENGTH"),
                TargetUnit = "INCHES"
            });
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
            var d = await Deserialize<ApiResponse<QuantityMeasurementDTO>>(r);
            Assert.AreEqual(12.0, d?.Data?.ResultValue);
        }

        [TestMethod]
        public async Task Convert_100Celsius_Returns212Fahrenheit()
        {
            Auth();
            var r = await Post("/api/v1/quantities/convert", new ConvertRequestDTO
            {
                ThisQuantityDTO = new QuantityDTO(100.0, "CELSIUS", "TEMPERATURE"),
                TargetUnit = "FAHRENHEIT"
            });
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
            var d = await Deserialize<ApiResponse<QuantityMeasurementDTO>>(r);
            Assert.AreEqual(212.0, d?.Data?.ResultValue);
        }

        // ── ADD ───────────────────────────────────────────────────────────

        [TestMethod]
        public async Task Add_1Feet_12Inches_Returns2Feet()
        {
            Auth();
            var r = await Post("/api/v1/quantities/add", new QuantityInputDTO
            {
                ThisQuantityDTO = new QuantityDTO(1.0, "FEET", "LENGTH"),
                ThatQuantityDTO = new QuantityDTO(12.0, "INCHES", "LENGTH")
            });
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
            var d = await Deserialize<ApiResponse<QuantityMeasurementDTO>>(r);
            Assert.AreEqual(2.0, d?.Data?.ResultValue);
            Assert.AreEqual("FEET", d?.Data?.ResultUnit);
        }

        // ── ERROR SCENARIOS ───────────────────────────────────────────────

        [TestMethod]
        public async Task Add_DifferentCategories_Returns400()
        {
            Auth();
            var r = await Post("/api/v1/quantities/add", new QuantityInputDTO
            {
                ThisQuantityDTO = new QuantityDTO(1.0, "FEET", "LENGTH"),
                ThatQuantityDTO = new QuantityDTO(1.0, "KILOGRAM", "WEIGHT")
            });
            Assert.AreEqual(HttpStatusCode.BadRequest, r.StatusCode);
        }

        [TestMethod]
        public async Task NoToken_Returns401()
        {
            _client.DefaultRequestHeaders.Authorization = null;
            var r = await _client.GetAsync("/api/v1/quantities/count/ADD");
            Assert.AreEqual(HttpStatusCode.Unauthorized, r.StatusCode);
        }

        // ── HISTORY ───────────────────────────────────────────────────────

        [TestMethod]
        public async Task GetHistoryByOperation_Returns200()
        {
            Auth();
            var r = await _client.GetAsync("/api/v1/quantities/history/operation/ADD");
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
        }

        [TestMethod]
        public async Task GetErrorHistory_Returns200()
        {
            Auth();
            var r = await _client.GetAsync("/api/v1/quantities/history/errors");
            Assert.AreEqual(HttpStatusCode.OK, r.StatusCode);
        }

        // ── HELPERS ───────────────────────────────────────────────────────

        private static async Task SignupAndLogin()
        {
            var email = $"int_{Guid.NewGuid():N}@test.com";
            var r = await Post("/api/v1/users/signup", new SignupRequestDTO
            {
                Username = "int_" + Guid.NewGuid().ToString()[..6],
                Email    = email,
                Password = "TestP@ss1"
            });
            var d = await Deserialize<ApiResponse<AuthResponseDTO>>(r);
            _token = d?.Data?.Token ?? string.Empty;
        }

        private static async Task<HttpResponseMessage> Post<T>(string url, T body)
        {
            var content = new StringContent(JsonSerializer.Serialize(body, _json), Encoding.UTF8, "application/json");
            return await _client.PostAsync(url, content);
        }

        private static async Task<T?> Deserialize<T>(HttpResponseMessage r)
            => JsonSerializer.Deserialize<T>(await r.Content.ReadAsStringAsync(), _json);

        private static void Auth()
            => _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
    }
}
