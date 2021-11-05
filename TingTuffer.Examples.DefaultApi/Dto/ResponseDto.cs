namespace TingTuffer.Examples.DefaultApi.Dto
{
    public struct ResponseDto
    {
        public bool Published { get; init; }
        public Guid Id { get; init; }
        public DateTime PublishedDate { get; private init; }

        public ResponseDto(Guid id, bool success)
            => (Id, Published, PublishedDate) = (id, success, DateTime.UtcNow);
    }
}
