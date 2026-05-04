using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLNH.Models;

namespace QLNH.DAL.Repositories
{
    public class ManagerRepository
    {
        private readonly AppDbContext _context;
        public ManagerRepository(AppDbContext context) { _context = context; }

        public DataTable GetRevenueSummary(DateTime start, DateTime end)
        {
            DataTable dt = new DataTable();
            string connString = _context.Database.GetDbConnection().ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_ThongKeDoanhThu", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TuNgay", start);
                    cmd.Parameters.AddWithValue("@DenNgay", end);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetTopSellingDishes()
        {
            DataTable dt = new DataTable();
            string connString = _context.Database.GetDbConnection().ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT * FROM vw_Top10MonBanChay", conn))
                {
                    cmd.CommandType = CommandType.Text;
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public DataTable GetCategoryRevenue(DateTime start, DateTime end)
        {
            DataTable dt = new DataTable();
            string connString = _context.Database.GetDbConnection().ConnectionString;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DoanhThuTheoNhom", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@TuNgay", start);
                    cmd.Parameters.AddWithValue("@DenNgay", end);

                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
            }
            return dt;
        }

        public async Task<List<Monan>> GetAllDishesAsync() =>
            await _context.Monans.Include(m => m.MaNhomNavigation).OrderByDescending(m => m.MaMon).ToListAsync();

        public async Task<Monan?> GetDishByIdAsync(int id) => await _context.Monans.FindAsync(id);

        public async Task AddDishAsync(Monan monan)
        {
            _context.Monans.Add(monan);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDishAsync(Monan monan)
        {
            _context.Monans.Update(monan);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDishAsync(int id)
        {
            var monan = await _context.Monans.FindAsync(id);
            if (monan != null)
            {
                _context.Monans.Remove(monan);
                await _context.SaveChangesAsync();
            }
        }
    }
}