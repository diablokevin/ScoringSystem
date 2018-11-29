namespace ScoringSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class multiscore : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Schedules", name: "JudgeId", newName: "Judge_Id");
            RenameIndex(table: "dbo.Schedules", name: "IX_JudgeId", newName: "IX_Judge_Id");
            CreateTable(
                "dbo.Scores",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TimeConsume = c.Time(precision: 7),
                        Mark = c.Double(),
                        JudgeTime = c.DateTime(),
                        ModifyTime = c.DateTime(),
                        ScheduleId = c.Int(nullable: false),
                        JudgeId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Judges", t => t.JudgeId)
                .ForeignKey("dbo.Schedules", t => t.ScheduleId, cascadeDelete: true)
                .Index(t => t.ScheduleId)
                .Index(t => t.JudgeId);
            
            DropColumn("dbo.Schedules", "TimeConsume");
            DropColumn("dbo.Schedules", "Score");
            DropColumn("dbo.Schedules", "JudgeTime");
            DropColumn("dbo.Schedules", "ModifyTime");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Schedules", "ModifyTime", c => c.DateTime());
            AddColumn("dbo.Schedules", "JudgeTime", c => c.DateTime());
            AddColumn("dbo.Schedules", "Score", c => c.Double());
            AddColumn("dbo.Schedules", "TimeConsume", c => c.Time(precision: 7));
            DropForeignKey("dbo.Scores", "ScheduleId", "dbo.Schedules");
            DropForeignKey("dbo.Scores", "JudgeId", "dbo.Judges");
            DropIndex("dbo.Scores", new[] { "JudgeId" });
            DropIndex("dbo.Scores", new[] { "ScheduleId" });
            DropTable("dbo.Scores");
            RenameIndex(table: "dbo.Schedules", name: "IX_Judge_Id", newName: "IX_JudgeId");
            RenameColumn(table: "dbo.Schedules", name: "Judge_Id", newName: "JudgeId");
        }
    }
}
