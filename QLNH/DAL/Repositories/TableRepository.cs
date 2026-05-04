using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLNH.Models;

namespace QLNH.DAL.Repositories
{
    public class TableRepository
    {
        private readonly AppDbContext _context;
        public TableRepository(AppDbContext context) => _context = context;

        public async Task<List<Banan>> LayDanhSachBanAsync()
        {
            return await _context.Banans.ToListAsync();
        }
    }
}