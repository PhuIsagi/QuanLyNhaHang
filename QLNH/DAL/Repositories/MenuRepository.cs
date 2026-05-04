using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QLNH.Models;

namespace QLNH.DAL.Repositories
{
    public class MenuRepository
    {
        private readonly AppDbContext _context;
        public MenuRepository(AppDbContext context) => _context = context;

        public async Task<List<Nhommon>> LayNhomMonAsync() => await _context.Nhommons.ToListAsync();
        public async Task<List<Monan>> LayMonAnAsync() => await _context.Monans.ToListAsync();
    }
}