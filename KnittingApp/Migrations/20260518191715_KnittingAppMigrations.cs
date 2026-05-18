using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KnittingApp.Migrations
{
    /// <inheritdoc />
    public partial class KnittingAppMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Drafts",
                columns: table => new
                {
                    DraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    DraftJson = table.Column<string>(type: "text", nullable: false),
                    EndPointY = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drafts", x => x.DraftId);
                });

            migrationBuilder.CreateTable(
                name: "Forms",
                columns: table => new
                {
                    formId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Parts = table.Column<List<string>>(type: "text[]", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Forms", x => x.formId);
                });

            migrationBuilder.CreateTable(
                name: "LoopReaders",
                columns: table => new
                {
                    LoopReaderId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CurrentIndex = table.Column<int>(type: "integer", nullable: false),
                    LoopMapId = table.Column<Guid>(type: "uuid", nullable: false),
                    SpentTime = table.Column<TimeSpan>(type: "interval", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoopReaders", x => x.LoopReaderId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Password = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "LoopMaps",
                columns: table => new
                {
                    LoopMapId = table.Column<Guid>(type: "uuid", nullable: false),
                    loopMapJson = table.Column<string>(type: "jsonb", nullable: false),
                    LoopsReaderId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoopMaps", x => x.LoopMapId);
                    table.ForeignKey(
                        name: "FK_LoopMaps_LoopReaders_LoopsReaderId",
                        column: x => x.LoopsReaderId,
                        principalTable: "LoopReaders",
                        principalColumn: "LoopReaderId");
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    ModelId = table.Column<Guid>(type: "uuid", nullable: false),
                    ModelName = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FrontLoopMapId = table.Column<Guid>(type: "uuid", nullable: false),
                    BackLoopMapId = table.Column<Guid>(type: "uuid", nullable: false),
                    SleeveLoopMapId = table.Column<Guid>(type: "uuid", nullable: false),
                    FrontDraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    BackDraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    SleeveDraftId = table.Column<Guid>(type: "uuid", nullable: false),
                    MeasuresJson = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.ModelId);
                    table.ForeignKey(
                        name: "FK_Models_Drafts_BackDraftId",
                        column: x => x.BackDraftId,
                        principalTable: "Drafts",
                        principalColumn: "DraftId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Models_Drafts_FrontDraftId",
                        column: x => x.FrontDraftId,
                        principalTable: "Drafts",
                        principalColumn: "DraftId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Models_Drafts_SleeveDraftId",
                        column: x => x.SleeveDraftId,
                        principalTable: "Drafts",
                        principalColumn: "DraftId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Models_LoopMaps_BackLoopMapId",
                        column: x => x.BackLoopMapId,
                        principalTable: "LoopMaps",
                        principalColumn: "LoopMapId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Models_LoopMaps_FrontLoopMapId",
                        column: x => x.FrontLoopMapId,
                        principalTable: "LoopMaps",
                        principalColumn: "LoopMapId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Models_LoopMaps_SleeveLoopMapId",
                        column: x => x.SleeveLoopMapId,
                        principalTable: "LoopMaps",
                        principalColumn: "LoopMapId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Models_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Schemas",
                columns: table => new
                {
                    SchemaId = table.Column<Guid>(type: "uuid", nullable: false),
                    SchemaName = table.Column<string>(type: "text", nullable: false),
                    SchemaImage = table.Column<string>(type: "text", nullable: false),
                    LoopMapId = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Schemas", x => x.SchemaId);
                    table.ForeignKey(
                        name: "FK_Schemas_LoopMaps_LoopMapId",
                        column: x => x.LoopMapId,
                        principalTable: "LoopMaps",
                        principalColumn: "LoopMapId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LoopMaps_LoopsReaderId",
                table: "LoopMaps",
                column: "LoopsReaderId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_BackDraftId",
                table: "Models",
                column: "BackDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_BackLoopMapId",
                table: "Models",
                column: "BackLoopMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_FrontDraftId",
                table: "Models",
                column: "FrontDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_FrontLoopMapId",
                table: "Models",
                column: "FrontLoopMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_ModelName_UserId",
                table: "Models",
                columns: new[] { "ModelName", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Models_SleeveDraftId",
                table: "Models",
                column: "SleeveDraftId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_SleeveLoopMapId",
                table: "Models",
                column: "SleeveLoopMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_UserId",
                table: "Models",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Schemas_LoopMapId",
                table: "Schemas",
                column: "LoopMapId");

            migrationBuilder.CreateIndex(
                name: "IX_Schemas_SchemaName_UserId",
                table: "Schemas",
                columns: new[] { "SchemaName", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Forms");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Schemas");

            migrationBuilder.DropTable(
                name: "Drafts");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "LoopMaps");

            migrationBuilder.DropTable(
                name: "LoopReaders");
        }
    }
}
