using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QuantityMeasurementApi.Controller;
using QuantityMeasurementBusinessLayer.Interface;
using QuantityMeasurementModel.Dto;

namespace QuantityMeasurementApi.Tests
{
    /// <summary>
    /// UC17: Unit tests for QuantityMeasurementController.
    /// IQuantityMeasurementApiService is mocked — only controller logic is tested.
    /// Spring equivalent: @WebMvcTest + @MockBean.
    /// </summary>
    [TestClass]
    public class QuantityMeasurementControllerTest
    {
        private Mock<IQuantityMeasurementApiService> _mockService = null!;
        private Mock<ILogger<QuantityMeasurementController>> _mockLogger = null!;
        private QuantityMeasurementController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockService = new Mock<IQuantityMeasurementApiService>();
            _mockLogger  = new Mock<ILogger<QuantityMeasurementController>>();
            _controller  = new QuantityMeasurementController(_mockService.Object, _mockLogger.Object);
        }

        [TestMethod]
        public async Task Compare_1Feet_12Inches_ReturnsEqual()
        {
            var input = new QuantityInputDTO
            {
                ThisQuantityDTO = new QuantityDTO(1.0, "FEET", "LENGTH"),
                ThatQuantityDTO = new QuantityDTO(12.0, "INCHES", "LENGTH")
            };
            var expected = new QuantityMeasurementDTO { OperationType = "COMPARE", ResultValue = 1, ResultUnit = "EQUAL" };
            _mockService.Setup(s => s.CompareAsync(input)).ReturnsAsync(expected);

            var result = await _controller.Compare(input) as OkObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            var response = result.Value as ApiResponse<QuantityMeasurementDTO>;
            Assert.IsTrue(response?.Success);
            Assert.AreEqual("EQUAL", response?.Data?.ResultUnit);
        }

        [TestMethod]
        public async Task Convert_1Feet_Returns12Inches()
        {
            var input = new ConvertRequestDTO
            {
                ThisQuantityDTO = new QuantityDTO(1.0, "FEET", "LENGTH"),
                TargetUnit = "INCHES"
            };
            var expected = new QuantityMeasurementDTO { OperationType = "CONVERT", ResultValue = 12.0, ResultUnit = "INCHES" };
            _mockService.Setup(s => s.ConvertAsync(input)).ReturnsAsync(expected);

            var result = await _controller.Convert(input) as OkObjectResult;

            var response = result?.Value as ApiResponse<QuantityMeasurementDTO>;
            Assert.AreEqual(12.0, response?.Data?.ResultValue);
            Assert.AreEqual("INCHES", response?.Data?.ResultUnit);
        }

        [TestMethod]
        public async Task Add_1Feet_12Inches_Returns2Feet()
        {
            var input = new QuantityInputDTO
            {
                ThisQuantityDTO = new QuantityDTO(1.0, "FEET", "LENGTH"),
                ThatQuantityDTO = new QuantityDTO(12.0, "INCHES", "LENGTH")
            };
            var expected = new QuantityMeasurementDTO { OperationType = "ADD", ResultValue = 2.0, ResultUnit = "FEET" };
            _mockService.Setup(s => s.AddAsync(input)).ReturnsAsync(expected);

            var result = await _controller.Add(input) as OkObjectResult;

            var response = result?.Value as ApiResponse<QuantityMeasurementDTO>;
            Assert.AreEqual(2.0, response?.Data?.ResultValue);
            Assert.AreEqual("FEET", response?.Data?.ResultUnit);
        }

        [TestMethod]
        public async Task Subtract_2Feet_6Inches_Returns1Point5Feet()
        {
            var input = new QuantityInputDTO
            {
                ThisQuantityDTO = new QuantityDTO(2.0, "FEET", "LENGTH"),
                ThatQuantityDTO = new QuantityDTO(6.0, "INCHES", "LENGTH")
            };
            var expected = new QuantityMeasurementDTO { OperationType = "SUBTRACT", ResultValue = 1.5, ResultUnit = "FEET" };
            _mockService.Setup(s => s.SubtractAsync(input)).ReturnsAsync(expected);

            var result = await _controller.Subtract(input) as OkObjectResult;

            var response = result?.Value as ApiResponse<QuantityMeasurementDTO>;
            Assert.AreEqual(1.5, response?.Data?.ResultValue);
        }

        [TestMethod]
        public async Task Divide_6Feet_2Feet_Returns3Ratio()
        {
            var input = new QuantityInputDTO
            {
                ThisQuantityDTO = new QuantityDTO(6.0, "FEET", "LENGTH"),
                ThatQuantityDTO = new QuantityDTO(2.0, "FEET", "LENGTH")
            };
            var expected = new QuantityMeasurementDTO { OperationType = "DIVIDE", ResultValue = 3.0, ResultUnit = "RATIO", ResultCategory = "SCALAR" };
            _mockService.Setup(s => s.DivideAsync(input)).ReturnsAsync(expected);

            var result = await _controller.Divide(input) as OkObjectResult;

            var response = result?.Value as ApiResponse<QuantityMeasurementDTO>;
            Assert.AreEqual(3.0, response?.Data?.ResultValue);
            Assert.AreEqual("RATIO", response?.Data?.ResultUnit);
        }

        [TestMethod]
        public async Task GetHistoryByOperation_ReturnsRecordList()
        {
            var history = new List<QuantityMeasurementDTO>
            {
                new() { OperationType = "ADD", ResultValue = 2.0 },
                new() { OperationType = "ADD", ResultValue = 5.0 }
            };
            _mockService.Setup(s => s.GetHistoryByOperationAsync("ADD")).ReturnsAsync(history);

            var result = await _controller.GetHistoryByOperation("ADD") as OkObjectResult;

            var response = result?.Value as ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>;
            Assert.AreEqual(2, response?.Data?.Count);
        }

        [TestMethod]
        public async Task GetOperationCount_ReturnsCorrectCount()
        {
            _mockService.Setup(s => s.GetOperationCountAsync("COMPARE")).ReturnsAsync(7);

            var result = await _controller.GetOperationCount("COMPARE") as OkObjectResult;

            var response = result?.Value as ApiResponse<int>;
            Assert.AreEqual(7, response?.Data);
        }

        [TestMethod]
        public async Task GetErrorHistory_ReturnsErrorRecordsOnly()
        {
            var errors = new List<QuantityMeasurementDTO>
            {
                new() { OperationType = "ADD", HasError = true, ErrorMessage = "Temperature does not support Add." }
            };
            _mockService.Setup(s => s.GetErrorHistoryAsync()).ReturnsAsync(errors);

            var result = await _controller.GetErrorHistory() as OkObjectResult;

            var response = result?.Value as ApiResponse<IReadOnlyList<QuantityMeasurementDTO>>;
            Assert.AreEqual(1, response?.Data?.Count);
            Assert.IsTrue(response?.Data?[0].HasError);
        }

        [TestMethod]
        public async Task Compare_CallsServiceExactlyOnce()
        {
            var input = new QuantityInputDTO
            {
                ThisQuantityDTO = new QuantityDTO(1.0, "FEET", "LENGTH"),
                ThatQuantityDTO = new QuantityDTO(1.0, "FEET", "LENGTH")
            };
            _mockService.Setup(s => s.CompareAsync(input)).ReturnsAsync(new QuantityMeasurementDTO());

            await _controller.Compare(input);

            _mockService.Verify(s => s.CompareAsync(input), Times.Once);
        }
    }
}
