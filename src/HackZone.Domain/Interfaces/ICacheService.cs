namespace HackZone.Domain.Interfaces;

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default);
    Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken ct = default);
    Task RemoveAsync(string key, CancellationToken ct = default);
    Task UpdateScoreAsync(string username, double score);
    Task<List<(string Username, double Score, long Rank)>> GetLeaderboardAsync(int top = 20);
}
