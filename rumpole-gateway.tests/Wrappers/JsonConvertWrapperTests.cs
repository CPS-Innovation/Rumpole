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
        private Fixture _fixture;

        public JsonConvertWrapperTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void SerializeObjectShouldDelegate()
        {
            var testObject = _fixture.Create<ResponseCaseDetails>();
            var expectedSerialisedRequest = JsonConvert.SerializeObject(testObject, Formatting.None, new JsonSerializerSettings());

            var serialisedObject = new JsonConvertWrapper().SerializeObject(testObject);

            serialisedObject.Should().BeEquivalentTo(expectedSerialisedRequest);
        }

        [Fact]
        public void DeserializeObjectShouldDelegate()
        {
            var testObject = _fixture.Create<ResponseCaseDetails>();
            var serialisedRequest = JsonConvert.SerializeObject(testObject, Formatting.None, new JsonSerializerSettings());

            var deserialisedRequest = new JsonConvertWrapper().DeserializeObject<ResponseCaseDetails>(serialisedRequest);

            testObject.Should().BeEquivalentTo(deserialisedRequest);
        }
    }
}
