using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksAndVideosShop.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ActiveMembership = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MembershipProducts",
                columns: table => new
                {
                    Id = table.Column<byte>(type: "INTEGER", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    MembershipType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MembershipProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PhysicalProducts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ProductType = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalProducts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerId = table.Column<long>(type: "INTEGER", nullable: false),
                    Total = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CustomerMembershipId = table.Column<byte>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK__PurchaseOrders_CustomerId__Customers_Id",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__PurchaseOrders_CustomerMembershipId__MembershipProducts_Id",
                        column: x => x.CustomerMembershipId,
                        principalTable: "MembershipProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderItemLines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PurchaseOrderId = table.Column<long>(type: "INTEGER", nullable: false),
                    PhysicalProductId = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderItemLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK__OrderItemLines_PhysicalProductId__PhysicalProducts_Id",
                        column: x => x.PhysicalProductId,
                        principalTable: "PhysicalProducts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK__OrderItemLines_PurchaseOrderId__PurchaseOrders_Id",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemLines_PhysicalProductId",
                table: "OrderItemLines",
                column: "PhysicalProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderItemLines_PurchaseOrderId",
                table: "OrderItemLines",
                column: "PurchaseOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CustomerId",
                table: "PurchaseOrders",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_CustomerMembershipId",
                table: "PurchaseOrders",
                column: "CustomerMembershipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderItemLines");

            migrationBuilder.DropTable(
                name: "PhysicalProducts");

            migrationBuilder.DropTable(
                name: "PurchaseOrders");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "MembershipProducts");
        }
    }
}
