namespace HackZone.Application.Features.Forum;

public record ForumCategoryItem(Guid Id, string Name, string Slug, string Description, int PostCount);
public record PostListItem(Guid Id, string Title, string AuthorUsername, string? AuthorAvatar,
    int ReplyCount, int ViewCount, DateTime CreatedAt);
public record PostDetail(Guid Id, string Title, string Body, string AuthorUsername,
    string? AuthorAvatar, int ViewCount, bool IsLocked, DateTime CreatedAt,
    List<ReplyItem> Replies);
public record ReplyItem(Guid Id, string Body, string AuthorUsername, string? AuthorAvatar, DateTime CreatedAt);
public record CreatePostRequest(Guid CategoryId, string Title, string Body);
public record CreateReplyRequest(string Body);
