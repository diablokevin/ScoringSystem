namespace ScoringSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class allowNullDataInSchedule : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Schedules", "PlanBeginTime", c => c.DateTime());
            AlterColumn("dbo.Schedules", "PlanEndTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Schedules", "PlanEndTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Schedules", "PlanBeginTime", c => c.DateTime(nullable: false));
        }
    }
}
