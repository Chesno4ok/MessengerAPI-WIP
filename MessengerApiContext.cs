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

    public virtual DbSet<ChatUser> ChatUsers { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("workstation id=MessengerAPI.mssql.somee.com;packet size=4096;user id=Wikok_SQLLogin_1;pwd=jid-htn-eNC-3H5;data source=MessengerAPI.mssql.somee.com;persist security info=False;initial catalog=MessengerAPI;TrustServerCertificate=true");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_Chats_1");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChatName).HasColumnName("chatName");
        });

        modelBuilder.Entity<ChatUser>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChatId).HasColumnName("chatId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.Chat).WithMany(p => p.ChatUsers)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chatId");

            entity.HasOne(d => d.User).WithMany(p => p.ChatUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userId");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.ToTable("Message");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.ChatId).HasColumnName("chatId");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.IsRead).HasColumnName("isRead");
            entity.Property(e => e.Type)
                .HasDefaultValueSql("((0))")
                .HasColumnName("type");
            entity.Property(e => e.User).HasColumnName("user");

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.Token).HasColumnName("token");
            entity.Property(e => e.Updates).HasColumnName("updates");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
