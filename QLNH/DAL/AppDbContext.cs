using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using QLNH.Models;

namespace QLNH.DAL;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Banan> Banans { get; set; }

    public virtual DbSet<Chitiethoadon> Chitiethoadons { get; set; }

    public virtual DbSet<Chitietphieugoi> Chitietphieugois { get; set; }

    public virtual DbSet<Hoadon> Hoadons { get; set; }

    public virtual DbSet<Monan> Monans { get; set; }

    public virtual DbSet<Nhanvien> Nhanviens { get; set; }

    public virtual DbSet<Nhommon> Nhommons { get; set; }

    public virtual DbSet<Phieugoi> Phieugois { get; set; }

    public virtual DbSet<Thongbao> Thongbaos { get; set; }

    public virtual DbSet<GopYKhachHang> GopYKhachHangs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Banan>(entity =>
        {
            entity.HasKey(e => e.SoBan).HasName("PK__banan__21B4EECABCF55A8E");

            entity.ToTable("banan");

            entity.Property(e => e.SoBan).ValueGeneratedNever();
            entity.Property(e => e.SoGhe).HasDefaultValue(4);
            entity.Property(e => e.Tang).HasDefaultValue(1);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("Trong");
        });

        modelBuilder.Entity<Chitiethoadon>(entity =>
        {
            entity.HasKey(e => e.MaChiTiet).HasName("PK__chitieth__CDF0A1148E922BDD");

            entity.ToTable("chitiethoadon");

            entity.Property(e => e.DonGia).HasColumnType("decimal(10, 0)");
            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.SoLuong).HasDefaultValue(1);
            entity.Property(e => e.ThoiGianGoi)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TrangThaiMon)
                .HasMaxLength(50)
                .HasDefaultValue("ChoCheBien");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.Chitiethoadons)
                .HasForeignKey(d => d.MaHoaDon)
                .HasConstraintName("FK__chitietho__MaHoa__5535A963");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.Chitiethoadons)
                .HasForeignKey(d => d.MaMon)
                .HasConstraintName("FK__chitietho__MaMon__5629CD9C");
        });

        modelBuilder.Entity<Chitietphieugoi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__chitietp__3214EC2753C6480D");

            entity.ToTable("chitietphieugoi");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.GhiChu).HasMaxLength(45);
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("ChoCheBien");

            entity.HasOne(d => d.MaMonNavigation).WithMany(p => p.Chitietphieugois)
                .HasForeignKey(d => d.MaMon)
                .HasConstraintName("FK__chitietph__MaMon__60A75C0F");

            entity.HasOne(d => d.MaPhieuNavigation).WithMany(p => p.Chitietphieugois)
                .HasForeignKey(d => d.MaPhieu)
                .HasConstraintName("FK__chitietph__MaPhi__5FB337D6");
        });

        modelBuilder.Entity<Hoadon>(entity =>
        {
            entity.HasKey(e => e.MaHoaDon).HasName("PK__hoadon__835ED13BA9339D9B");

            entity.ToTable("hoadon");

            entity.Property(e => e.GhiChu).HasMaxLength(255);
            entity.Property(e => e.GiamGia)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 0)");
            entity.Property(e => e.MaNvPhucVu).HasColumnName("MaNV_PhucVu");
            entity.Property(e => e.ThoiGianRa).HasColumnType("datetime");
            entity.Property(e => e.ThoiGianVao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TienKhachDua)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 0)");
            entity.Property(e => e.TienThua)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 0)");
            entity.Property(e => e.TongThanhToan)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 0)");
            entity.Property(e => e.TongTienHang)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 0)");
            entity.Property(e => e.TrangThai)
                .HasMaxLength(50)
                .HasDefaultValue("ChuaThanhToan");
            entity.Property(e => e.Vat)
                .HasDefaultValue(0m)
                .HasColumnType("decimal(10, 0)")
                .HasColumnName("VAT");

            entity.HasOne(d => d.MaNvPhucVuNavigation).WithMany(p => p.Hoadons)
                .HasForeignKey(d => d.MaNvPhucVu)
                .HasConstraintName("FK__hoadon__MaNV_Phu__4AB81AF0");

            entity.HasOne(d => d.SoBanNavigation).WithMany(p => p.Hoadons)
                .HasForeignKey(d => d.SoBan)
                .HasConstraintName("FK__hoadon__SoBan__49C3F6B7");
        });

        modelBuilder.Entity<Monan>(entity =>
        {
            entity.HasKey(e => e.MaMon).HasName("PK__monan__3A5B29A892498510");

            entity.ToTable("monan");

            entity.Property(e => e.DangKinhDoanh).HasDefaultValue(true);
            entity.Property(e => e.DonVi).HasMaxLength(50);
            entity.Property(e => e.GiaTien).HasColumnType("decimal(10, 0)");
            entity.Property(e => e.MaCode).HasMaxLength(20);
            entity.Property(e => e.TenMon).HasMaxLength(100);

            entity.HasOne(d => d.MaNhomNavigation).WithMany(p => p.Monans)
                .HasForeignKey(d => d.MaNhom)
                .HasConstraintName("FK__monan__MaNhom__45F365D3");
        });

        modelBuilder.Entity<Nhanvien>(entity =>
        {
            entity.HasKey(e => e.MaNv).HasName("PK__nhanvien__2725D70A2239DDA3");

            entity.ToTable("nhanvien");

            entity.HasIndex(e => e.TenDangNhap, "UQ__nhanvien__55F68FC030D9FD10").IsUnique();

            entity.Property(e => e.MaNv).HasColumnName("MaNV");
            entity.Property(e => e.HoTen).HasMaxLength(100);
            entity.Property(e => e.MatKhau).HasMaxLength(255);
            entity.Property(e => e.NgayTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.TenDangNhap).HasMaxLength(50);
            entity.Property(e => e.VaiTro).HasMaxLength(50);
        });

        modelBuilder.Entity<Nhommon>(entity =>
        {
            entity.HasKey(e => e.MaNhom).HasName("PK__nhommon__234F91CD680A5E30");

            entity.ToTable("nhommon");

            entity.Property(e => e.TenNhom).HasMaxLength(50);
        });

        modelBuilder.Entity<Phieugoi>(entity =>
        {
            entity.HasKey(e => e.MaPhieu).HasName("PK__phieugoi__2660BFE078CFFCF0");

            entity.ToTable("phieugoi");

            entity.Property(e => e.ThoiGianTao)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.MaHoaDonNavigation).WithMany(p => p.Phieugois)
                .HasForeignKey(d => d.MaHoaDon)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__phieugoi__MaHoaD__5BE2A6F2");
        });

        modelBuilder.Entity<Thongbao>(entity =>
        {
            entity.HasKey(e => e.MaTb).HasName("PK__thongbao__2725006F892C146F");

            entity.ToTable("thongbao");

            entity.Property(e => e.MaTb).HasColumnName("MaTB");
            entity.Property(e => e.DaXem).HasDefaultValue(false);
            entity.Property(e => e.NoiDung).HasMaxLength(255);
            entity.Property(e => e.ThoiGian)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}