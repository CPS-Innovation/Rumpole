using System.Linq;
using AutoFixture;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using RumpoleGateway.Domain.RumpolePipeline;
using RumpoleGateway.Mappers;
using Xunit;

namespace RumpoleGateway.Tests.Mappers
{
    public class StreamlinedSearchWordMapperTests
    {
        private readonly Fixture _fixture;
        private readonly string _searchTerm;

        public StreamlinedSearchWordMapperTests()
        {
            _fixture = new Fixture();

            _searchTerm = _fixture.Create<string>();
        }

        [Fact]
        public void GivenASearchWordThatMatchesTheSearchTerm_ThenTheMapperProvidesAllDetail()
        {
            var searchLine = _fixture.Create<SearchLine>();
            searchLine.Words = _fixture.CreateMany<Word>(1).ToList();
            searchLine.Words[0].Text = _searchTerm;

            IStreamlinedSearchWordMapper mapper = new StreamlinedSearchWordMapper();
            var result = mapper.Map(searchLine.Words[0], _searchTerm);

            using (new AssertionScope())
            {
                result.Text.Should().Be(_searchTerm);
                result.BoundingBox.Should().BeEquivalentTo(searchLine.Words[0].BoundingBox);
            }
        }
        
        [Fact]
        public void GivenASearchWordThatDoesNotMatchTheSearchTerm_ThenTheMapperProvidesNoBoundingBoxDetail()
        {
            var searchLine = _fixture.Create<SearchLine>();
            searchLine.Words = _fixture.CreateMany<Word>(1).ToList();
            
            IStreamlinedSearchWordMapper mapper = new StreamlinedSearchWordMapper();
            var result = mapper.Map(searchLine.Words[0], _searchTerm);

            result.BoundingBox.Should().BeNull();
        }
        
        [Theory]
        [InlineData(" police. ", "police")]
        [InlineData(" police, ", "police")]
        [InlineData(" police! ", "police")]
        public void GivenASearchTermThatIsFoundInASentenceWithPunctuation_ThenTheMapperProvidesDetail(string searchResultText, string searchTerm)
        {
            var searchLine = _fixture.Create<SearchLine>();
            searchLine.Words = _fixture.CreateMany<Word>(1).ToList();
            searchLine.Words[0].Text = searchResultText;
            
            IStreamlinedSearchWordMapper mapper = new StreamlinedSearchWordMapper();
            var result = mapper.Map(searchLine.Words[0], searchTerm);

            result.BoundingBox.Should().NotBeNull();
        }
    }
}
