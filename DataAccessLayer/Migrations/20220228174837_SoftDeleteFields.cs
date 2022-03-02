using System;
using Microsoft.EntityFrameworkCore.Migrations;
using DataAccessLayer.Entities;

namespace DataAccessLayer.Migrations
{
    public partial class SoftDeleteFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductRating_AspNetUsers_UserId",
                table: "ProductRating");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductRating_Products_ProductId",
                table: "ProductRating");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductRating",
                table: "ProductRating");

            migrationBuilder.RenameTable(
                name: "ProductRating",
                newName: "Ratings");

            migrationBuilder.RenameIndex(
                name: "IX_ProductRating_UserId",
                table: "Ratings",
                newName: "IX_Ratings_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductRating_ProductId",
                table: "Ratings",
                newName: "IX_Ratings_ProductId");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Products",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Ratings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Ratings_Products_ProductId",
                table: "Ratings",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            foreach (Platforms platform in Enum.GetValues(typeof(Platforms)))
            {
                migrationBuilder.InsertData("Platforms", "PlatformName", Platform.Name(platform));
            }
            migrationBuilder.InsertData("Genres", "GenreName", "Shooter");
            migrationBuilder.InsertData("Genres", "GenreName", "Strategy");
            migrationBuilder.InsertData("Products", new string[] { "Name", "PlatformId", "GenreId", "TotalRating", "DateCreated", "IsDeleted" }, new object[] { "Game1", 6, 1, 9, DateTime.Now, false });
            migrationBuilder.InsertData("Products", new string[] { "Name", "PlatformId", "GenreId", "TotalRating", "DateCreated", "IsDeleted" }, new object[] { "Game2", 6, 1, 9, DateTime.Now, false });
            migrationBuilder.InsertData("Products", new string[] { "Name", "PlatformId", "GenreId", "TotalRating", "DateCreated", "IsDeleted" }, new object[] { "Game3", 6, 2, 9, DateTime.Now, false });
            migrationBuilder.InsertData("Products", new string[] { "Name", "PlatformId", "GenreId", "TotalRating", "DateCreated", "IsDeleted" }, new object[] { "Game4", 6, 1, 9, DateTime.Now, false });
            migrationBuilder.InsertData("Products", new string[] { "Name", "PlatformId", "GenreId", "TotalRating", "DateCreated", "IsDeleted" }, new object[] { "Game5", 3, 2, 9, DateTime.Now, false });
            migrationBuilder.InsertData("Products", new string[] { "Name", "PlatformId", "GenreId", "TotalRating", "DateCreated", "IsDeleted" }, new object[] { "Some1", 3, 1, 9, DateTime.Now, false });
            migrationBuilder.InsertData("Products", new string[] { "Name", "PlatformId", "GenreId", "TotalRating", "DateCreated", "IsDeleted" }, new object[] { "Some2", 3, 2, 9, DateTime.Now, false });
            migrationBuilder.InsertData("Products", new string[] { "Name", "PlatformId", "GenreId", "TotalRating", "DateCreated", "IsDeleted" }, new object[] { "Some3", 5, 1, 9, DateTime.Now, false });
            migrationBuilder.InsertData("Products", new string[] { "Name", "PlatformId", "GenreId", "TotalRating", "DateCreated", "IsDeleted" }, new object[] { "Emag1", 5, 2, 9, DateTime.Now, false });
            migrationBuilder.InsertData("Products", new string[] { "Name", "PlatformId", "GenreId", "TotalRating", "DateCreated", "IsDeleted" }, new object[] { "Emag2", 5, 1, 9, DateTime.Now, false });
            migrationBuilder.InsertData("Products", new string[] { "Name", "PlatformId", "GenreId", "TotalRating", "DateCreated", "IsDeleted" }, new object[] { "GaSo1", 5, 1, 9, DateTime.Now, false });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_AspNetUsers_UserId",
                table: "Ratings");

            migrationBuilder.DropForeignKey(
                name: "FK_Ratings_Products_ProductId",
                table: "Ratings");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Ratings",
                table: "Ratings");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Ratings");

            migrationBuilder.RenameTable(
                name: "Ratings",
                newName: "ProductRating");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_UserId",
                table: "ProductRating",
                newName: "IX_ProductRating_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Ratings_ProductId",
                table: "ProductRating",
                newName: "IX_ProductRating_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductRating",
                table: "ProductRating",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductRating_AspNetUsers_UserId",
                table: "ProductRating",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductRating_Products_ProductId",
                table: "ProductRating",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
