using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazorREPRODEV.App.Server.Migrations
{
    public partial class addexamrecordlist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExamRecordId",
                table: "ExamStudentRecord",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ExamStudentRecord_ExamRecordId",
                table: "ExamStudentRecord",
                column: "ExamRecordId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamStudentRecord_ExamRecords_ExamRecordId",
                table: "ExamStudentRecord",
                column: "ExamRecordId",
                principalTable: "ExamRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamStudentRecord_ExamRecords_ExamRecordId",
                table: "ExamStudentRecord");

            migrationBuilder.DropIndex(
                name: "IX_ExamStudentRecord_ExamRecordId",
                table: "ExamStudentRecord");

            migrationBuilder.DropColumn(
                name: "ExamRecordId",
                table: "ExamStudentRecord");
        }
    }
}
