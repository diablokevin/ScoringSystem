namespace ScoringSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyschedule : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Schedules", "CompetitorId", "dbo.Competitors");
            DropForeignKey("dbo.Schedules", "JudgeId", "dbo.Judges");
            DropIndex("dbo.Schedules", new[] { "CompetitorId" });
            DropIndex("dbo.Schedules", new[] { "JudgeId" });
            AlterColumn("dbo.Schedules", "CompetitorId", c => c.Int());
            AlterColumn("dbo.Schedules", "JudgeId", c => c.Int());
            CreateIndex("dbo.Schedules", "CompetitorId");
            CreateIndex("dbo.Schedules", "JudgeId");
            AddForeignKey("dbo.Schedules", "CompetitorId", "dbo.Competitors", "Id");
            AddForeignKey("dbo.Schedules", "JudgeId", "dbo.Judges", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Schedules", "JudgeId", "dbo.Judges");
            DropForeignKey("dbo.Schedules", "CompetitorId", "dbo.Competitors");
            DropIndex("dbo.Schedules", new[] { "JudgeId" });
            DropIndex("dbo.Schedules", new[] { "CompetitorId" });
            AlterColumn("dbo.Schedules", "JudgeId", c => c.Int(nullable: false));
            AlterColumn("dbo.Schedules", "CompetitorId", c => c.Int(nullable: false));
            CreateIndex("dbo.Schedules", "JudgeId");
            CreateIndex("dbo.Schedules", "CompetitorId");
            AddForeignKey("dbo.Schedules", "JudgeId", "dbo.Judges", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Schedules", "CompetitorId", "dbo.Competitors", "Id", cascadeDelete: true);
        }
    }
}
