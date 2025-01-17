﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class TrnMonthlyRepayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "trn_monthly_repayment",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    repayment_id = table.Column<string>(type: "text", nullable: false),
                    amount = table.Column<decimal>(type: "numeric", nullable: false),
                    status = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_trn_monthly_repayment", x => x.id);
                    table.ForeignKey(
                        name: "FK_trn_monthly_repayment_trn_repayment_repayment_id",
                        column: x => x.repayment_id,
                        principalTable: "trn_repayment",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_trn_monthly_repayment_repayment_id",
                table: "trn_monthly_repayment",
                column: "repayment_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "trn_monthly_repayment");
        }
    }
}
