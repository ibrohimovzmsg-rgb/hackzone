using System.Text.Json;
using HackZone.Domain.Interfaces;
using StackExchange.Redis;

namespace HackZone.Infrastructure.Services;

public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
{
    private readonly IDatabase _db = redis.GetDatabase();
    private const string LeaderboardKey = "hackzone:leaderboard";

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
    {
        var value = await _db.StringGetAsync(key);
        return value.HasValue ? JsonSerializer.Deserialize<T>(value!) : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiry);
    }

    public async Task RemoveAsync(string key, CancellationToken ct = default) =>
        await _db.KeyDeleteAsync(key);

    public async Task UpdateScoreAsync(string username, double score) =>
        await _db.SortedSetAddAsync(LeaderboardKey, username, score);

    public async Task<List<(string Username, double Score, long Rank)>> GetLeaderboardAsync(int top = 20)
    {
        var entries = await _db.SortedSetRangeByRankWithScoresAsync(
            LeaderboardKey, 0, top - 1, Order.Descending);
        return entries.Select((e, i) => (e.Element.ToString(), e.Score, (long)(i + 1))).ToList();
    }
}
