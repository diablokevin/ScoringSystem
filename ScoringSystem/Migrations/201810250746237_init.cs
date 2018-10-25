namespace ScoringSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Competitors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Pro = c.String(),
                        StaffId = c.String(),
                        Race_num = c.String(),
                        CompanyId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.Schedules",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        PlanBeginTime = c.DateTime(nullable: false),
                        PlanEndTime = c.DateTime(nullable: false),
                        TimeConsume = c.Time(precision: 7),
                        Score = c.Double(),
                        JudgeTime = c.DateTime(),
                        ModifyTime = c.DateTime(),
                        EventId = c.Int(nullable: false),
                        CompetitorId = c.Int(nullable: false),
                        JudgeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Competitors", t => t.CompetitorId, cascadeDelete: true)
                .ForeignKey("dbo.Events", t => t.EventId, cascadeDelete: true)
                .ForeignKey("dbo.Judges", t => t.JudgeId, cascadeDelete: true)
                .Index(t => t.EventId)
                .Index(t => t.CompetitorId)
                .Index(t => t.JudgeId);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Pro = c.String(),
                        MenuOrder = c.Int(nullable: false),
                        TimeLimit = c.Time(nullable: false, precision: 7),
                        ChipId = c.Int(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Judges",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        CompanyId = c.Int(),
                        EventId = c.Int(),
                        StaffId = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Companies", t => t.CompanyId)
                .ForeignKey("dbo.Events", t => t.EventId)
                .Index(t => t.CompanyId)
                .Index(t => t.EventId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Schedules", "JudgeId", "dbo.Judges");
            DropForeignKey("dbo.Judges", "EventId", "dbo.Events");
            DropForeignKey("dbo.Judges", "CompanyId", "dbo.Companies");
            DropForeignKey("dbo.Schedules", "EventId", "dbo.Events");
            DropForeignKey("dbo.Schedules", "CompetitorId", "dbo.Competitors");
            DropForeignKey("dbo.Competitors", "CompanyId", "dbo.Companies");
            DropIndex("dbo.Judges", new[] { "EventId" });
            DropIndex("dbo.Judges", new[] { "CompanyId" });
            DropIndex("dbo.Schedules", new[] { "JudgeId" });
            DropIndex("dbo.Schedules", new[] { "CompetitorId" });
            DropIndex("dbo.Schedules", new[] { "EventId" });
            DropIndex("dbo.Competitors", new[] { "CompanyId" });
            DropTable("dbo.Judges");
            DropTable("dbo.Events");
            DropTable("dbo.Schedules");
            DropTable("dbo.Competitors");
            DropTable("dbo.Companies");
        }
    }
}
