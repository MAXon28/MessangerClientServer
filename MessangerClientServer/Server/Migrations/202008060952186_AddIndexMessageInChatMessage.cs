namespace Server.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddIndexMessageInChatMessage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatMessages", "IndexMessage", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatMessages", "IndexMessage");
        }
    }
}