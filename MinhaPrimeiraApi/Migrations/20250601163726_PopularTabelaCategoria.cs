using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MinhaPrimeiraApi.Migrations
{
    /// <inheritdoc />
    public partial class PopularTabelaCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("INSERT INTO Categories (Name, ImageUrl) VALUES ('Bebidas', 'https://careclub.com.br/wp-content/uploads/2022/09/As-Principais-Diferenc%CC%A7as-entre-as-Bebidas-Alcoo%CC%81licas-1024x683.jpg')");
            mb.Sql("INSERT INTO Categories (Name, ImageUrl) VALUES ('Lanches', 'https://instadelivery-public.nyc3.cdn.digitaloceanspaces.com/groups/170740153165c4e13be96e1.jpeg')");
            mb.Sql("INSERT INTO Categories (Name, ImageUrl) VALUES ('Sobremesas', 'https://ellebrasil-wp-images.s3.amazonaws.com/2022/09/image-288.jpg')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("DELETE FROM Categories");
        }
    }
}
