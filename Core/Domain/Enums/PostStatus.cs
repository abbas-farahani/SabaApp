namespace Core.Domain.Enums;

public enum PostStatus : byte
{
    deleted = 0,
    published = 1,
    edited = 2,
    draft = 3,
    @private = 4,
    scheduled = 5
}
