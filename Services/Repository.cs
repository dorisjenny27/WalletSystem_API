using Microsoft.EntityFrameworkCore;
using WalletSystem.Data;
using WalletSystem.Models.Entity;
using WalletSystem.Services.Interfaces;

namespace WalletSystem.Services
{
    public class Repository : IRepository
    {
        private readonly AppDbContext _context;
        public Repository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<bool> AddAsync<T>(T entity) where T : class
        {
            _context.Add(entity);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public async Task<bool> DeleteAsync<T>(T entity) where T : class
        {
            _context.Remove(entity);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }

        public IQueryable<T> GetAllAsync<T>() where T : class
        {
            return _context.Set<T>();
        }

        public async Task<T?> GetByIdAsync<T>(string Id) where T : class
        {
            return await _context.FindAsync<T>(Id);

        }

        public async Task<TotalBalance?> GetTotalBalanceByUserIdAsync(string userId)
        {
            return await _context.TotalBalances.FirstOrDefaultAsync(tb => tb.UserId == userId);
        }


        public async Task<bool> UpdateAsync<T>(T entity) where T : class
        {
            _context.Update(entity);
            return await _context.SaveChangesAsync() > 0 ? true : false;
        }
 

        public async Task<Wallet?> GetByCurrencyAsync(string userId, string currency)
        {
            return await _context.Wallets
                .Include(w => w.User)
                .FirstOrDefaultAsync(w => w.UserId == userId && w.CurrencyCode == currency);
        }
    }
}

