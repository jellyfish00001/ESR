using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ERS.Migrations
{
    public partial class CreateCashPaymentHead : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "cash_payment_head",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    sysno = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "付款流水号"),
                    seq = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, comment: "seq"),
                    bank = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, comment: "银行名称"),
                    payment_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "付款日期"),
                    amt = table.Column<decimal>(type: "numeric(18,2)", nullable: false, comment: "金额"),
                    identification = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "Payment Run會計"),
                    payment_status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "Payment状态"),
                    payment_docno = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true, comment: "付款传票号"),
                    stutus = table.Column<string>(type: "character varying(1)", maxLength: 1, nullable: true, comment: "状态"),
                    assigned_emplid = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, comment: "应签核人工号"),
                    assigned_name = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true, comment: "签核人名字"),
                    approver_emplid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "實際簽核人工號"),
                    approver_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true, comment: "實際簽核人姓名"),
                    approver_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true, comment: "签核时间"),
                    approver_remark = table.Column<DateTime>(type: "timestamp without time zone", maxLength: 200, nullable: true, comment: "签核意见"),
                    mdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    cdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    cuser = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    muser = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    isdeleted = table.Column<bool>(type: "boolean", nullable: false),
                    company = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cash_payment_head", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "sysno_idx1",
                table: "cash_payment_head",
                column: "sysno");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "cash_payment_head");
        }
    }
}
