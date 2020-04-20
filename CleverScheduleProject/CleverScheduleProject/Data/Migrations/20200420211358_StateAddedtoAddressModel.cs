using Microsoft.EntityFrameworkCore.Migrations;

namespace CleverScheduleProject.Migrations
{
    public partial class StateAddedtoAddressModel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Clients_ClientId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Contractors_ContractorId",
                table: "Appointments");

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "Addresses",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Clients_ClientId",
                table: "Appointments",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Contractors_ContractorId",
                table: "Appointments",
                column: "ContractorId",
                principalTable: "Contractors",
                principalColumn: "ContractorId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Clients_ClientId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Contractors_ContractorId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "State",
                table: "Addresses");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Clients_ClientId",
                table: "Appointments",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "ClientId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Contractors_ContractorId",
                table: "Appointments",
                column: "ContractorId",
                principalTable: "Contractors",
                principalColumn: "ContractorId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
