using MongoDB.Driver;
using CommBank.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CommBank.Services
{
    public class GoalsService : IGoalsService
    {
        private readonly IMongoCollection<Goal> _goalsCollection;

        public GoalsService(IMongoDatabase database)
        {
            _goalsCollection = database.GetCollection<Goal>("Goals");
        }

        public async Task<List<Goal>> GetAsync()
        {
            var goals = await _goalsCollection.Find(goal => true).ToListAsync();
            Console.WriteLine($"📝 [DEBUG] Found {goals.Count} goals in the database");
            return goals;
        }


        public async Task<Goal?> GetAsync(string id) =>
            await _goalsCollection.Find(goal => goal.Id == id).FirstOrDefaultAsync();

        public async Task<List<Goal>?> GetForUserAsync(string userId) =>
            await _goalsCollection.Find(goal => goal.UserId == userId).ToListAsync();

        public async Task CreateAsync(Goal newGoal)
        {
            await _goalsCollection.InsertOneAsync(newGoal);
            Console.WriteLine($"✅ Inserted new goal: {newGoal.Name}");
        }

        public async Task UpdateAsync(string id, Goal updatedGoal) =>
            await _goalsCollection.ReplaceOneAsync(goal => goal.Id == id, updatedGoal);

        public async Task RemoveAsync(string id) =>
            await _goalsCollection.DeleteOneAsync(goal => goal.Id == id);
    }
}