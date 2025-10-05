using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace CookingRecipe.Models;

public partial class CookingrecipeContext : DbContext
{
    public CookingrecipeContext()
    {
    }

    public CookingrecipeContext(DbContextOptions<CookingrecipeContext> options)
        : base(options)
    {
    }
    public virtual DbSet<Role> Roles { get; set; }
    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Favorite> Favorites { get; set; }

    public virtual DbSet<Ingredient> Ingredients { get; set; }

    public virtual DbSet<Recipe> Recipes { get; set; }

    public virtual DbSet<RecipeCategory> RecipeCategories { get; set; }

    public virtual DbSet<RecipeIngredient> RecipeIngredients { get; set; }

    public virtual DbSet<Recipestep> Recipesteps { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.CategoryId).HasName("PRIMARY");

            entity.ToTable("categories");

            entity.HasIndex(e => e.Name, "idx_name");

            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Favorite>(entity =>
        {
            entity.HasKey(e => e.FavoriteId).HasName("PRIMARY");

            entity.ToTable("favorites");

            entity.HasIndex(e => e.RecipeId, "idx_recipe");

            entity.HasIndex(e => e.UserId, "idx_user");

            entity.HasIndex(e => new { e.UserId, e.RecipeId }, "unique_user_recipe").IsUnique();

            entity.Property(e => e.FavoriteId).HasColumnName("favorite_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("favorites_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Favorites)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("favorites_ibfk_1");
        });

        modelBuilder.Entity<Ingredient>(entity =>
        {
            entity.HasKey(e => e.IngredientId).HasName("PRIMARY");

            entity.ToTable("ingredients");

            entity.HasIndex(e => e.Name, "idx_name");

            entity.HasIndex(e => e.Type, "idx_type");

            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Type)
                .HasDefaultValueSql("'other'")
                .HasColumnType("enum('dry','fresh','spice','dairy','meat','seafood','vegetable','fruit','other')")
                .HasColumnName("type");
        });

        modelBuilder.Entity<Recipe>(entity =>
        {
            entity.HasKey(e => e.RecipeId).HasName("PRIMARY");

            entity.ToTable("recipes");

            entity.HasIndex(e => e.AuthorId, "idx_author");

            entity.HasIndex(e => e.Difficulty, "idx_difficulty");

            entity.HasIndex(e => new { e.Title, e.Description }, "idx_search").HasAnnotation("MySql:FullTextIndex", true);

            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.AuthorId).HasColumnName("author_id");
            entity.Property(e => e.CookTime)
                .HasComment("Thời gian nấu (phút)")
                .HasColumnName("cook_time");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Difficulty)
                .HasDefaultValueSql("'medium'")
                .HasColumnType("enum('easy','medium','hard')")
                .HasColumnName("difficulty");
            entity.Property(e => e.Instructions)
                .HasColumnType("text")
                .HasColumnName("instructions");
            entity.Property(e => e.PrepTime)
                .HasComment("Thời gian chuẩn bị (phút)")
                .HasColumnName("prep_time");
            entity.Property(e => e.Title)
                .HasMaxLength(200)
                .HasColumnName("title");

            entity.HasOne(d => d.Author).WithMany(p => p.Recipes)
                .HasForeignKey(d => d.AuthorId)
                .HasConstraintName("recipes_ibfk_1");
        });

        modelBuilder.Entity<RecipeCategory>(entity =>
        {
            entity.HasKey(e => e.RecipeCategoryId).HasName("PRIMARY");

            entity.ToTable("recipe_categories");

            entity.HasIndex(e => e.CategoryId, "idx_category");

            entity.HasIndex(e => e.RecipeId, "idx_recipe");

            entity.HasIndex(e => new { e.RecipeId, e.CategoryId }, "unique_recipe_category").IsUnique();

            entity.Property(e => e.RecipeCategoryId).HasColumnName("recipe_category_id");
            entity.Property(e => e.CategoryId).HasColumnName("category_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");

            entity.HasOne(d => d.Category).WithMany(p => p.RecipeCategories)
                .HasForeignKey(d => d.CategoryId)
                .HasConstraintName("recipe_categories_ibfk_2");

            entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeCategories)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("recipe_categories_ibfk_1");
        });

        modelBuilder.Entity<RecipeIngredient>(entity =>
        {
            entity.HasKey(e => e.RecipeIngredientId).HasName("PRIMARY");

            entity.ToTable("recipe_ingredients");

            entity.HasIndex(e => e.IngredientId, "idx_ingredient");

            entity.HasIndex(e => e.RecipeId, "idx_recipe");

            entity.Property(e => e.RecipeIngredientId).HasColumnName("recipe_ingredient_id");
            entity.Property(e => e.IngredientId).HasColumnName("ingredient_id");
            entity.Property(e => e.Quantity)
                .HasMaxLength(50)
                .HasComment("vd: 200g, 2 muỗng canh")
                .HasColumnName("quantity");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");

            entity.HasOne(d => d.Ingredient).WithMany(p => p.RecipeIngredients)
                .HasForeignKey(d => d.IngredientId)
                .HasConstraintName("recipe_ingredients_ibfk_2");

            entity.HasOne(d => d.Recipe).WithMany(p => p.RecipeIngredients)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("recipe_ingredients_ibfk_1");
        });

        modelBuilder.Entity<Recipestep>(entity =>
        {
            entity.HasKey(e => e.StepId).HasName("PRIMARY");

            entity.ToTable("recipesteps");

            entity.HasIndex(e => e.RecipeId, "idx_recipe");

            entity.Property(e => e.StepId).HasColumnName("step_id");
            entity.Property(e => e.Content)
                .HasColumnType("text")
                .HasColumnName("content");
            entity.Property(e => e.Duration)
                .HasComment("Thời gian của bước này (phút)")
                .HasColumnName("duration");
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(255)
                .HasColumnName("image_url");
            entity.Property(e => e.RecipeId).HasColumnName("recipe_id");
            entity.Property(e => e.StepNumber).HasColumnName("step_number");
            entity.Property(e => e.VideoUrl)
                .HasMaxLength(255)
                .HasColumnName("video_url");

            entity.HasOne(d => d.Recipe).WithMany(p => p.Recipesteps)
                .HasForeignKey(d => d.RecipeId)
                .HasConstraintName("recipesteps_ibfk_1");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.HasIndex(e => e.Email, "email").IsUnique();

            entity.HasIndex(e => e.Username, "idx_username").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP")
                .HasColumnType("timestamp")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(100)
                .HasColumnName("full_name");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
