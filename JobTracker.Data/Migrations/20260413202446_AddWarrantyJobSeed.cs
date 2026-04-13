using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JobTracker.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddWarrantyJobSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Clients",
                columns: new[] { "Id", "CompanyName", "ContactName", "PhoneNumber" },
                values: new object[] { 3, "Deloitte", "Melanie Scott", "0209846573" });

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "EmployeeCode", "FullName" },
                values: new object[] { 3, "EMP003", "Robin McGee" });

            migrationBuilder.InsertData(
                table: "Jobs",
                columns: new[] { "Id", "ClientId", "Date", "JobNo", "JobType" },
                values: new object[] { 3, 3, new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "W000001", 2 });

            migrationBuilder.InsertData(
                table: "TimeCards",
                columns: new[] { "Id", "DateWorked", "EmployeeId", "HoursWorked", "JobId" },
                values: new object[] { 3, new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 6.0m, 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "TimeCards",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Jobs",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Clients",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
