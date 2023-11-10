using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ChesnokMessengerAPI;

public partial class MessengerApiContext : DbContext
{
    public MessengerApiContext()
    {
    }

    public MessengerApiContext(DbContextOptions<MessengerApiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("workstation id=MessengerAPI.mssql.somee.com;packet size=4096;user id=Wikok_SQLLogin_1;pwd=jid-htn-eNC-3H5;data source=MessengerAPI.mssql.somee.com;persist security info=False;initial catalog=MessengerAPI;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ChatId)
                .IsUnicode(false)
                .HasColumnName("chatId");
            entity.Property(e => e.User).HasColumnName("user");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.Chat)
                .HasForeignKey<Chat>(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_User");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Message");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.ChatId)
                .IsUnicode(false)
                .HasColumnName("chatId");
            entity.Property(e => e.Content)
                .IsUnicode(false)
                .HasColumnName("content");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.FromUser).HasColumnName("fromUser");

            entity.HasOne(d => d.FromUserNavigation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.FromUser)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.HasUpdates).HasColumnName("hasUpdates");
            entity.Property(e => e.Login)
                .IsUnicode(false)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.UserName)
                .IsUnicode(false)
                .HasColumnName("userName");
            entity.Property(e => e.UserToken)
                .IsUnicode(false)
                .HasColumnName("userToken");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
