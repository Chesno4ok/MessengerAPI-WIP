using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ChesnokMessengerAPI;

public partial class MessengerContext : DbContext
{
    public MessengerContext()
    {
    }

    public MessengerContext(DbContextOptions<MessengerContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Chat> Chats { get; set; }

    public virtual DbSet<ChatUser> ChatUsers { get; set; }

    public virtual DbSet<File> Files { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Token> Tokens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=193.124.44.234;Port=5432;Database=Messenger;Username=postgres;Password=HGdsfe3as");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Chat>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_chats_1");
        });

        modelBuilder.Entity<ChatUser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_chatusers");

            entity.Property(e => e.HasUpdates).HasDefaultValue((short)0);

            entity.HasOne(d => d.Chat).WithMany(p => p.ChatUsers)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("chatid");

            entity.HasOne(d => d.User).WithMany(p => p.ChatUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("userid");
        });

        modelBuilder.Entity<File>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Files_pkey");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_message");

            entity.Property(e => e.Date).HasPrecision(3);
            entity.Property(e => e.IsRead).HasDefaultValue((short)0);

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.Messages)
                .HasForeignKey(d => d.User)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.Id).ValueGeneratedOnAdd();

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_FK");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("pk_users");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
