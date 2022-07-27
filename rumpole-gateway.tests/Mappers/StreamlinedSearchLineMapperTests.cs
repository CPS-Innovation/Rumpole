using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Mappers;
using Xunit;

namespace RumpoleGateway.Tests.Mappers
{
    public class StreamlinedSearchLineMapperTests
    {
        private readonly Fixture _fixture;

        public StreamlinedSearchLineMapperTests()
        {
            _fixture = new Fixture();
        }

        [Fact]
        public void GivenASearchLine_ReturnAStreamlinedVersion()
        {
            var searchLine = _fixture.Create<SearchLine>();

            IStreamlinedSearchLineMapper mapper = new StreamlinedSearchLineMapper();
            var streamlinedVersion = mapper.Map(searchLine);

            using (new AssertionScope())
            {
                streamlinedVersion.DocumentId.Should().Be(searchLine.DocumentId);
                streamlinedVersion.CaseId.Should().Be(searchLine.CaseId);
                streamlinedVersion.Id.Should().Be(searchLine.Id);
                streamlinedVersion.LineIndex.Should().Be(searchLine.LineIndex);
                streamlinedVersion.PageIndex.Should().Be(searchLine.PageIndex);
                streamlinedVersion.Text.Should().Be(searchLine.Text);
            }
        }
    }
}
