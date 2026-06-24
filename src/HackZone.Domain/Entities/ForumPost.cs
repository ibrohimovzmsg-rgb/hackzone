using HackZone.Domain.Common;
using HackZone.Domain.Enums;

namespace HackZone.Domain.Entities;

public class ForumCategory : BaseEntity
{
    public string Name { get; private set; } = default!;
    public string Slug { get; private set; } = default!;
    public string Description { get; private set; } = default!;
    public int OrderIndex { get; private set; }

    private ForumCategory() { }

    public static ForumCategory Create(string name, string description, int order) => new()
    {
        Name = name,
        Slug = name.ToLower().Replace(" ", "-"),
        Description = description,
        OrderIndex = order
    };
}

public class ForumPost : BaseEntity
{
    public Guid CategoryId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Title { get; private set; } = default!;
    public string Body { get; private set; } = default!;
    public ForumPostType Type { get; private set; }
    public bool IsLocked { get; private set; }
    public bool IsDeleted { get; private set; }
    public int ViewCount { get; private set; }
    public ForumCategory Category { get; private set; } = default!;
    public User Author { get; private set; } = default!;

    private readonly List<ForumReply> _replies = [];
    public IReadOnlyCollection<ForumReply> Replies => _replies.AsReadOnly();

    private ForumPost() { }

    public static ForumPost Create(Guid categoryId, Guid authorId, string title, string body,
        ForumPostType type = ForumPostType.Discussion) => new()
    {
        CategoryId = categoryId, AuthorId = authorId,
        Title = title, Body = body, Type = type
    };

    public void Lock() { IsLocked = true; SetUpdated(); }
    public void SoftDelete() { IsDeleted = true; SetUpdated(); }
    public void IncrementView() { ViewCount++; }
}

public class ForumReply : BaseEntity
{
    public Guid PostId { get; private set; }
    public Guid AuthorId { get; private set; }
    public string Body { get; private set; } = default!;
    public bool IsDeleted { get; private set; }
    public ForumPost Post { get; private set; } = default!;
    public User Author { get; private set; } = default!;

    private ForumReply() { }

    public static ForumReply Create(Guid postId, Guid authorId, string body) =>
        new() { PostId = postId, AuthorId = authorId, Body = body };

    public void SoftDelete() { IsDeleted = true; SetUpdated(); }
}
