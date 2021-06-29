using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazorREPRODEV.App.Server.Migrations
{
    public partial class recreatesometables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExamStudentRecord_ExamRecords_ExamRecordId",
                table: "ExamStudentRecord");

            migrationBuilder.DropForeignKey(
                name: "FK_ExamStudentRecord_Questions_QuestionId",
                table: "ExamStudentRecord");

            migrationBuilder.DropIndex(
                name: "IX_ExamStudentRecord_ExamRecordId",
                table: "ExamStudentRecord");

            migrationBuilder.DropIndex(
                name: "IX_ExamStudentRecord_QuestionId",
                table: "ExamStudentRecord");

            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                table: "ExamStudentRecord",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ExamRecordId",
                table: "ExamStudentRecord",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "QuestionId",
                table: "ExamStudentRecord",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ExamRecordId",
                table: "ExamStudentRecord",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_ExamStudentRecord_ExamRecordId",
                table: "ExamStudentRecord",
                column: "ExamRecordId");

            migrationBuilder.CreateIndex(
                name: "IX_ExamStudentRecord_QuestionId",
                table: "ExamStudentRecord",
                column: "QuestionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExamStudentRecord_ExamRecords_ExamRecordId",
                table: "ExamStudentRecord",
                column: "ExamRecordId",
                principalTable: "ExamRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ExamStudentRecord_Questions_QuestionId",
                table: "ExamStudentRecord",
                column: "QuestionId",
                principalTable: "Questions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
