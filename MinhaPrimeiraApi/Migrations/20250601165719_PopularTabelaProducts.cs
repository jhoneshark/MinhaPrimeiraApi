using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaPrimeiraApi.Migrations
{
    /// <inheritdoc />
    public partial class PopularTabelaProducts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO Products (Name, Description, Price, ImageUrl, Stock, CreatedAt, CategoryId) " + 
                   "VALUES ('Coca Cola', 'Coca Cola 2L', 10.50, 'https://www.bloomberglinea.com.br/resizer/dWFMtnkpq1QmXn7MCUiJgWOb7-E=/arc-photo-bloomberglinea/arc2-prod/public/C3BQMW4VTNAYHBRSBD2X7TEW6Q.jpeg', 100, now(), 1)");
            
            mb.Sql("INSERT INTO Products (Name, Description, Price, ImageUrl, Stock, CreatedAt, CategoryId) " +
                   "VALUES ('McNífico Bacon', 'Um hambúrguer (carne 100% bovina), bacon', 45.50, 'https://api-middleware-mcd.mcdonaldscupones.com/media/image/product$kzXjtUHf/200/200/original?country=br', 100, now(), 2)");
            
            mb.Sql("INSERT INTO Products (Name, Description, Price, ImageUrl, Stock, CreatedAt, CategoryId) " +
                   "VALUES ('Pudim', 'Doce de leite', 15.50, 'https://i0.wp.com/www.receitasqueamo.com.br/wp-content/uploads/2019/12/IMG_9918.jpg?resize=700%2C390&ssl=1', 100, now(), 3)");
            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("DELETE FROM Products");
        }
    }
}
