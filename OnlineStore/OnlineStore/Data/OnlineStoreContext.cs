using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using OnlineStore.DBModels;

namespace OnlineStore.Data;

public partial class OnlineStoreContext : DbContext
{
    public OnlineStoreContext()
    {
    }

    public OnlineStoreContext(DbContextOptions<OnlineStoreContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Address> Addresses { get; set; }

    public virtual DbSet<Brand> Brands { get; set; }

    public virtual DbSet<CardDetail> CardDetails { get; set; }

    public virtual DbSet<Cart> Carts { get; set; }

    public virtual DbSet<CartItem> CartItems { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<OrderItem> OrderItems { get; set; }

    public virtual DbSet<OrderPayment> OrderPayments { get; set; }

    public virtual DbSet<OrderPaymentStatus> OrderPaymentStatuses { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=devsql\\sql2012;Database=TrainingIleanaR_OnlineStore;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Address>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Address__3214EC276A2489AB");

            entity.ToTable("Address");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Apartment).HasColumnName("apartment");
            entity.Property(e => e.BuildingName)
                .HasMaxLength(50)
                .HasColumnName("buildingName");
            entity.Property(e => e.City)
                .HasMaxLength(255)
                .HasColumnName("city");
            entity.Property(e => e.Entrace)
                .HasMaxLength(50)
                .HasColumnName("entrace");
            entity.Property(e => e.FloorNumber).HasColumnName("floorNumber");
            entity.Property(e => e.Street)
                .HasMaxLength(255)
                .HasColumnName("street");
            entity.Property(e => e.StreetNumber)
                .HasMaxLength(50)
                .HasColumnName("streetNumber");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(5)
                .HasColumnName("zipCode");
        });

        modelBuilder.Entity<Brand>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Brand__3214EC273A1B0A20");

            entity.ToTable("Brand");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BrandName)
                .HasMaxLength(1)
                .HasColumnName("brandName");
        });

        modelBuilder.Entity<CardDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CardDeta__3214EC27D806E51A");

            entity.HasIndex(e => e.CardNumber, "UQ__CardDeta__4CD3FAA2B22A8442").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CardNumber)
                .HasMaxLength(50)
                .HasColumnName("cardNumber");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithMany(p => p.CardDetails)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_CardDetails_Users");
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cart__3214EC27D49172CC");

            entity.ToTable("Cart");

            entity.HasIndex(e => e.UserId, "UQ__Cart__CB9A1CDE84A9AF71").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.User).WithOne(p => p.Cart)
                .HasForeignKey<Cart>(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Cart__userID__276EDEB3");
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__CartItem__3214EC27226763E4");

            entity.ToTable("CartItem");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CartId).HasColumnName("cartID");
            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.Quantity).HasColumnName("quantity");

            entity.HasOne(d => d.Cart).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.CartId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItem__cartID__2A4B4B5E");

            entity.HasOne(d => d.Product).WithMany(p => p.CartItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CartItem__produc__2B3F6F97");
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Category__3214EC2784FCA0AE");

            entity.ToTable("Category");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CategoryName)
                .HasMaxLength(1)
                .HasColumnName("categoryName");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Customer__3214EC274390F093");

            entity.ToTable("Customer");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AddressId).HasColumnName("addressID");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .HasColumnName("lastName");
            entity.Property(e => e.Phone)
                .HasMaxLength(24)
                .HasColumnName("phone");

            entity.HasOne(d => d.Address).WithMany(p => p.Customers)
                .HasForeignKey(d => d.AddressId)
                .HasConstraintName("FK__Customer__addres__1DE57479");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Orders__3214EC27189C8F2B");

            entity.HasIndex(e => e.OrderNumber, "UQ__Orders__6296129F7BEA039F").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CustomerId).HasColumnName("customerID");
            entity.Property(e => e.OrderDate)
                .HasPrecision(3)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("orderDate");
            entity.Property(e => e.OrderNumber)
                .HasMaxLength(50)
                .HasColumnName("orderNumber");
            entity.Property(e => e.StatusId).HasColumnName("statusID");

            entity.HasOne(d => d.Customer).WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__customer__300424B4");

            entity.HasOne(d => d.Status).WithMany(p => p.Orders)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStatus_Orders");
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderIte__3214EC2752688B96");

            entity.ToTable("OrderItem");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Discount)
                .HasColumnType("decimal(5, 2)")
                .HasColumnName("discount");
            entity.Property(e => e.OrderId).HasColumnName("orderID");
            entity.Property(e => e.ProductId).HasColumnName("productID");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.UnitPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("unitPrice");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__order__34C8D9D1");

            entity.HasOne(d => d.Product).WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderItem__produ__33D4B598");
        });

        modelBuilder.Entity<OrderPayment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderPay__3214EC2785B345E4");

            entity.ToTable("OrderPayment");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CardDetailsId).HasColumnName("cardDetailsID");
            entity.Property(e => e.OrderId).HasColumnName("orderID");
            entity.Property(e => e.PaymentDate)
                .HasPrecision(3)
                .HasColumnName("paymentDate");
            entity.Property(e => e.PaytmentMethod)
                .HasMaxLength(25)
                .HasColumnName("paytmentMethod");
            entity.Property(e => e.StatusId).HasColumnName("statusID");
            entity.Property(e => e.TotalAmount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("totalAmount");

            entity.HasOne(d => d.CardDetails).WithMany(p => p.OrderPayments)
                .HasForeignKey(d => d.CardDetailsId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderPaym__cardD__440B1D61");

            entity.HasOne(d => d.Order).WithMany(p => p.OrderPayments)
                .HasForeignKey(d => d.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__OrderPaym__order__4316F928");

            entity.HasOne(d => d.Status).WithMany(p => p.OrderPayments)
                .HasForeignKey(d => d.StatusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OrderStatus_OrderPayment");
        });

        modelBuilder.Entity<OrderPaymentStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderPay__3214EC27869B795E");

            entity.ToTable("OrderPaymentStatus");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
        });

        modelBuilder.Entity<OrderStatus>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__OrderSta__3214EC277F48A948");

            entity.ToTable("OrderStatus");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Product__3214EC27FF9A39AA");

            entity.ToTable("Product");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BrandId).HasColumnName("brandID");
            entity.Property(e => e.CategoryId).HasColumnName("categoryID");
            entity.Property(e => e.CreatedAt)
                .HasPrecision(3)
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnName("createdAt");
            entity.Property(e => e.CreatedBy)
                .HasMaxLength(1)
                .HasColumnName("createdBy");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("imageURL");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Stock).HasColumnName("stock");

            entity.HasOne(d => d.Brand).WithMany(p => p.Products)
                .HasForeignKey(d => d.BrandId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__brandID__164452B1");

            entity.HasOne(d => d.Category).WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__categor__15502E78");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC27F802FB4B");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164EE8A5155").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC572DDB8A982").IsUnique();

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CustomerId).HasColumnName("customerID");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.IsActive).HasColumnName("isActive");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("passwordHash");
            entity.Property(e => e.RoleId).HasColumnName("roleID");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");

            entity.HasOne(d => d.Customer).WithMany(p => p.Users)
                .HasForeignKey(d => d.CustomerId)
                .HasConstraintName("FK__Users__customerI__22AA2996");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_UserRole_Users");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__UserStat__3214EC277DDE97B4");

            entity.ToTable("UserRole");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
