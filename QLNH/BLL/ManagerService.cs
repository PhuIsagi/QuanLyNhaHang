using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using QLNH.DAL.Repositories;
using QLNH.Models;

namespace QLNH.BLL
{
    public class ManagerService
    {
        private readonly ManagerRepository _repo;
        public ManagerService(ManagerRepository repo) { _repo = repo; }

        public async Task<object> GetRevenueReportAsync(DateTime start, DateTime end)
        {
            return await Task.Run(() =>
            {
                end = end.Date.AddDays(1).AddTicks(-1);

                DataTable dtSummary = _repo.GetRevenueSummary(start, end);
                decimal totalRevenue = 0;
                int totalInvoices = 0;

                if (dtSummary.Rows.Count > 0)
                {
                    totalRevenue = dtSummary.Rows[0]["TongDoanhThu"] != DBNull.Value ? Convert.ToDecimal(dtSummary.Rows[0]["TongDoanhThu"]) : 0;
                    totalInvoices = dtSummary.Rows[0]["TongSoHoaDon"] != DBNull.Value ? Convert.ToInt32(dtSummary.Rows[0]["TongSoHoaDon"]) : 0;
                }
                decimal avgRevenue = totalInvoices > 0 ? totalRevenue / totalInvoices : 0;

                DataTable dtTopDishes = _repo.GetTopSellingDishes();
                var topDishes = new List<object>();
                foreach (DataRow row in dtTopDishes.Rows)
                {
                    topDishes.Add(new
                    {
                        dish_name = row["TenMon"].ToString(),
                        quantity = Convert.ToInt32(row["TongSoLuongBan"])
                    });
                }

                DataTable dtCategory = _repo.GetCategoryRevenue(start, end);
                var categoryReport = new List<object>();
                foreach (DataRow row in dtCategory.Rows)
                {
                    categoryReport.Add(new
                    {
                        category = row["TenNhom"].ToString(),
                        revenue = Convert.ToDecimal(row["DoanhThu"])
                    });
                }

                return new
                {
                    success = true,
                    summary = new { total_revenue = totalRevenue, total_invoices = totalInvoices, avg_revenue_per_invoice = avgRevenue, report_period = $"{start:dd/MM} - {end:dd/MM}" },
                    category_report = categoryReport,
                    top_dishes = topDishes
                };
            });
        }

        public async Task<object> GetAllDishesAsync()
        {
            var rawList = await _repo.GetAllDishesAsync();

            var cleanList = rawList.Select(d => new {
                maMon = d.MaMon,
                tenMon = d.TenMon,
                maNhom = d.MaNhom,
                giaTien = d.GiaTien,
                hinhAnh = d.HinhAnh
            }).ToList();

            return cleanList;
        }

        public async Task AddDishAsync(Monan monan) => await _repo.AddDishAsync(monan);

        public async Task UpdateDishAsync(int id, Monan dishUpdate)
        {
            var dish = await _repo.GetDishByIdAsync(id);
            if (dish != null)
            {
                dish.TenMon = dishUpdate.TenMon;
                dish.MaNhom = dishUpdate.MaNhom;
                dish.GiaTien = dishUpdate.GiaTien;
                dish.HinhAnh = dishUpdate.HinhAnh;
                await _repo.UpdateDishAsync(dish);
            }
        }

        public async Task DeleteDishAsync(int id) => await _repo.DeleteDishAsync(id);
    }
}