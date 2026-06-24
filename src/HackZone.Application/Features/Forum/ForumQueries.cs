using HackZone.Application.Common.Exceptions;
using HackZone.Domain.Entities;
using FluentValidation;
using HackZone.Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HackZone.Application.Features.Forum;

public record GetCategoriesQuery : IRequest<List<ForumCategoryItem>>;

public class GetCategoriesHandler(IUnitOfWork uow)
    : IRequestHandler<GetCategoriesQuery, List<ForumCategoryItem>>
{
    public async Task<List<ForumCategoryItem>> Handle(GetCategoriesQuery _, CancellationToken ct)
    {
        var cats = await uow.ForumCategories.Query()
            .OrderBy(c => c.OrderIndex).ToListAsync(ct);
        var postCounts = await uow.ForumPosts.Query()
            .Where(p => !p.IsDeleted)
            .GroupBy(p => p.CategoryId)
            .Select(g => new { g.Key, Count = g.Count() })
            .ToListAsync(ct);
        var countMap = postCounts.ToDictionary(x => x.Key, x => x.Count);

        return cats.Select(c => new ForumCategoryItem(
            c.Id, c.Name, c.Slug, c.Description,
            countMap.GetValueOrDefault(c.Id, 0))).ToList();
    }
}

public record GetPostsQuery(Guid? CategoryId = null, int Page = 1, int PageSize = 20) : IRequest<List<PostListItem>>;

public class GetPostsHandler(IUnitOfWork uow)
    : IRequestHandler<GetPostsQuery, List<PostListItem>>
{
    public async Task<List<PostListItem>> Handle(GetPostsQuery q, CancellationToken ct)
    {
        var query = uow.ForumPosts.Query()
            .Include(p => p.Author)
            .Include(p => p.Replies)
            .Where(p => !p.IsDeleted);

        if (q.CategoryId.HasValue)
            query = query.Where(p => p.CategoryId == q.CategoryId.Value);

        var posts = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((q.Page - 1) * q.PageSize).Take(q.PageSize)
            .ToListAsync(ct);

        return posts.Select(p => new PostListItem(
            p.Id, p.Title, p.Author.Username, p.Author.AvatarUrl,
            p.Replies.Count(r => !r.IsDeleted), p.ViewCount, p.CreatedAt)).ToList();
    }
}

public record GetPostDetailQuery(Guid PostId) : IRequest<PostDetail>;

public class GetPostDetailHandler(IUnitOfWork uow)
    : IRequestHandler<GetPostDetailQuery, PostDetail>
{
    public async Task<PostDetail> Handle(GetPostDetailQuery q, CancellationToken ct)
    {
        var post = await uow.ForumPosts.Query()
            .Include(p => p.Author)
            .Include(p => p.Replies.Where(r => !r.IsDeleted))
                .ThenInclude(r => r.Author)
            .FirstOrDefaultAsync(p => p.Id == q.PostId && !p.IsDeleted, ct)
            ?? throw new NotFoundException("Post", q.PostId);

        post.IncrementView();
        await uow.SaveChangesAsync(ct);

        return new PostDetail(
            post.Id, post.Title, post.Body,
            post.Author.Username, post.Author.AvatarUrl,
            post.ViewCount, post.IsLocked, post.CreatedAt,
            post.Replies.OrderBy(r => r.CreatedAt)
                .Select(r => new ReplyItem(r.Id, r.Body, r.Author.Username, r.Author.AvatarUrl, r.CreatedAt))
                .ToList());
    }
}

public record CreatePostCommand(Guid CategoryId, string Title, string Body) : IRequest<Guid>;

public class CreatePostValidator : AbstractValidator<CreatePostCommand>
{
    public CreatePostValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MinimumLength(5).MaximumLength(200);
        RuleFor(x => x.Body).NotEmpty().MinimumLength(10).MaximumLength(10000);
    }
}

public class CreatePostHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<CreatePostCommand, Guid>
{
    public async Task<Guid> Handle(CreatePostCommand cmd, CancellationToken ct)
    {
        if (currentUser.UserId is null) throw new UnauthorizedException();

        var post = ForumPost.Create(cmd.CategoryId, currentUser.UserId.Value, cmd.Title, cmd.Body);
        await uow.ForumPosts.AddAsync(post, ct);
        await uow.SaveChangesAsync(ct);
        return post.Id;
    }
}

public record CreateReplyCommand(Guid PostId, string Body) : IRequest<Guid>;

public class CreateReplyValidator : AbstractValidator<CreateReplyCommand>
{
    public CreateReplyValidator()
    {
        RuleFor(x => x.Body).NotEmpty().MinimumLength(5).MaximumLength(5000);
    }
}

public class CreateReplyHandler(IUnitOfWork uow, ICurrentUserService currentUser)
    : IRequestHandler<CreateReplyCommand, Guid>
{
    public async Task<Guid> Handle(CreateReplyCommand cmd, CancellationToken ct)
    {
        if (currentUser.UserId is null) throw new UnauthorizedException();

        var reply = ForumReply.Create(cmd.PostId, currentUser.UserId.Value, cmd.Body);
        await uow.ForumReplies.AddAsync(reply, ct);
        await uow.SaveChangesAsync(ct);
        return reply.Id;
    }
}
