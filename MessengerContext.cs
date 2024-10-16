using System;
using System.Collections.Generic;
using System.Configuration;
using ChesnokMessengerAPI.Models;
using ChesnokMessengerAPI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using File = ChesnokMessengerAPI.Models.File;

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
    {
        var config = AppConfiguration.GetInstance();
        var connectionString = config.Configuration.GetConnectionString("MessengerDatabase");

        optionsBuilder.UseNpgsql(connectionString);
    }
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

            entity.HasOne(d => d.Chat).WithMany(p => p.Messages)
                .HasForeignKey(d => d.ChatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_chatId");

            entity.HasOne(d => d.User).WithMany(p => p.Messages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_users");
        });

        modelBuilder.Entity<Token>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Tokens_pkey");

            entity.HasOne(d => d.User).WithMany(p => p.Tokens)
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
