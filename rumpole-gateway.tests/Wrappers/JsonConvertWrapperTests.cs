using AutoFixture;
using FluentAssertions;
using Newtonsoft.Json;
using RumpoleGateway.Domain.CoreDataApi.ResponseTypes;
using RumpoleGateway.Wrappers;
using Xunit;

namespace RumpoleGateway.tests.Wrappers
{
    public class JsonConvertWrapperTests
    {
        private readonly Fixture _fixture;

        public JsonConvertWrapperTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void SerializeObjectShouldDelegate()
        {
            var testObject = _fixture.Create<ResponseCaseDetails>();
            var expectedSerializedRequest = JsonConvert.SerializeObject(testObject, Formatting.None, new JsonSerializerSettings());

            var serializedObject = new JsonConvertWrapper().SerializeObject(testObject);

            serializedObject.Should().BeEquivalentTo(expectedSerializedRequest);
        }

        [Fact]
        public void DeserializeObjectShouldDelegate()
        {
            var testObject = _fixture.Create<ResponseCaseDetails>();
            var serializedRequest = JsonConvert.SerializeObject(testObject, Formatting.None, new JsonSerializerSettings());

            var deserializedRequest = new JsonConvertWrapper().DeserializeObject<ResponseCaseDetails>(serializedRequest);

            testObject.Should().BeEquivalentTo(deserializedRequest);
        }
    }
}
