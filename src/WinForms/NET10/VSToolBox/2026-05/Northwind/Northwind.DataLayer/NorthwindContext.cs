using Microsoft.EntityFrameworkCore;

namespace Northwind.DataLayer;

public class NorthwindContext : DbContext
{
    public NorthwindContext()
    {
    }

    public NorthwindContext(DbContextOptions<NorthwindContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Category> Categories { get; set; }
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<Employee> Employees { get; set; }
    public virtual DbSet<Order> Orders { get; set; }
    public virtual DbSet<OrderDetail> OrderDetails { get; set; }
    public virtual DbSet<Product> Products { get; set; }
    public virtual DbSet<Shipper> Shippers { get; set; }
    public virtual DbSet<Supplier> Suppliers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=Northwind;Integrated Security=true;TrustServerCertificate=true");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId);
            entity.Property(e => e.CategoryName)
                .IsRequired()
                .HasMaxLength(15);
            entity.Property(e => e.Description)
                .HasMaxLength(500);
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId);
            entity.Property(e => e.CustomerId)
                .HasMaxLength(5);
            entity.Property(e => e.CompanyName)
                .IsRequired()
                .HasMaxLength(40);
            entity.Property(e => e.ContactName)
                .HasMaxLength(30);
            entity.Property(e => e.ContactTitle)
                .HasMaxLength(30);
            entity.Property(e => e.Address)
                .HasMaxLength(60);
            entity.Property(e => e.City)
                .HasMaxLength(15);
            entity.Property(e => e.Region)
                .HasMaxLength(15);
            entity.Property(e => e.PostalCode)
                .HasMaxLength(10);
            entity.Property(e => e.Country)
                .HasMaxLength(15);
            entity.Property(e => e.Phone)
                .HasMaxLength(24);
            entity.Property(e => e.Fax)
                .HasMaxLength(24);
        });

        modelBuilder.Entity<Employee>(entity =>
        {
            entity.HasKey(e => e.EmployeeId);
            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(20);
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(10);
            entity.Property(e => e.Title)
                .HasMaxLength(30);
            entity.Property(e => e.TitleOfCourtesy)
                .HasMaxLength(25);
            entity.Property(e => e.Address)
                .HasMaxLength(60);
            entity.Property(e => e.City)
                .HasMaxLength(15);
            entity.Property(e => e.Region)
                .HasMaxLength(15);
            entity.Property(e => e.PostalCode)
                .HasMaxLength(10);
            entity.Property(e => e.Country)
                .HasMaxLength(15);
            entity.Property(e => e.HomePhone)
                .HasMaxLength(24);
            entity.Property(e => e.Extension)
                .HasMaxLength(4);
            entity.Property(e => e.Notes)
                .HasMaxLength(600);

            entity.HasOne(d => d.ReportsToNavigation)
                .WithMany(p => p.InverseReportsToNavigation)
                .HasForeignKey(d => d.ReportsTo);
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.OrderId);
            entity.Property(e => e.ShipName)
                .HasMaxLength(40);
            entity.Property(e => e.ShipAddress)
                .HasMaxLength(60);
            entity.Property(e => e.ShipCity)
                .HasMaxLength(15);
            entity.Property(e => e.ShipRegion)
                .HasMaxLength(15);
            entity.Property(e => e.ShipPostalCode)
                .HasMaxLength(10);
            entity.Property(e => e.ShipCountry)
                .HasMaxLength(15);

            entity.HasOne(d => d.Customer)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.CustomerId);

            entity.HasOne(d => d.Employee)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.EmployeeId);

            entity.HasOne(d => d.Shipper)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.ShipVia);
        });

        modelBuilder.Entity<OrderDetail>(entity =>
        {
            entity.HasKey(e => new { e.OrderId, e.ProductId });
            entity.ToTable("Order Details");

            entity.Property(e => e.UnitPrice)
                .HasColumnType("money");

            entity.HasOne(d => d.Order)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.OrderId);

            entity.HasOne(d => d.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.ProductId);
            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(40);
            entity.Property(e => e.QuantityPerUnit)
                .HasMaxLength(20);
            entity.Property(e => e.UnitPrice)
                .HasColumnType("money");

            entity.HasOne(d => d.Category)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.CategoryId);

            entity.HasOne(d => d.Supplier)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.SupplierId);
        });

        modelBuilder.Entity<Shipper>(entity =>
        {
            entity.HasKey(e => e.ShipperId);
            entity.Property(e => e.CompanyName)
                .IsRequired()
                .HasMaxLength(40);
            entity.Property(e => e.Phone)
                .HasMaxLength(24);
        });

        modelBuilder.Entity<Supplier>(entity =>
        {
            entity.HasKey(e => e.SupplierId);
            entity.Property(e => e.CompanyName)
                .IsRequired()
                .HasMaxLength(40);
            entity.Property(e => e.ContactName)
                .HasMaxLength(30);
            entity.Property(e => e.ContactTitle)
                .HasMaxLength(30);
            entity.Property(e => e.Address)
                .HasMaxLength(60);
            entity.Property(e => e.City)
                .HasMaxLength(15);
            entity.Property(e => e.Region)
                .HasMaxLength(15);
            entity.Property(e => e.PostalCode)
                .HasMaxLength(10);
            entity.Property(e => e.Country)
                .HasMaxLength(15);
            entity.Property(e => e.Phone)
                .HasMaxLength(24);
            entity.Property(e => e.Fax)
                .HasMaxLength(24);
            entity.Property(e => e.HomePage)
                .HasMaxLength(200);
        });
    }
}
