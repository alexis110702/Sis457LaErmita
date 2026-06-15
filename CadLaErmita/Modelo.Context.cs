namespace CadLaErmita
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;

    public partial class LabLaErmitaEntities : DbContext
    {
        public LabLaErmitaEntities()
            : base("name=LabLaErmitaEntities")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }

        public virtual DbSet<Rol>          Rol          { get; set; }
        public virtual DbSet<Usuario>      Usuario      { get; set; }
        public virtual DbSet<Categoria>    Categoria    { get; set; }
        public virtual DbSet<Producto>     Producto     { get; set; }
        public virtual DbSet<Venta>        Venta        { get; set; }
        public virtual DbSet<DetalleVenta> DetalleVenta { get; set; }

        public virtual ObjectResult<paUsuarioLogin_Result> paUsuarioLogin(string usuario, string contrasena)
        {
            var u = usuario    != null ? new ObjectParameter("usuario",    usuario)    : new ObjectParameter("usuario",    typeof(string));
            var c = contrasena != null ? new ObjectParameter("contrasena", contrasena) : new ObjectParameter("contrasena", typeof(string));
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<paUsuarioLogin_Result>("paUsuarioLogin", u, c);
        }

        public virtual ObjectResult<paCategoriaListar_Result> paCategoriaListar(string parametro)
        {
            var p = parametro != null ? new ObjectParameter("parametro", parametro) : new ObjectParameter("parametro", typeof(string));
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<paCategoriaListar_Result>("paCategoriaListar", p);
        }

        public virtual ObjectResult<paProductoListar_Result> paProductoListar(string parametro)
        {
            var p = parametro != null ? new ObjectParameter("parametro", parametro) : new ObjectParameter("parametro", typeof(string));
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<paProductoListar_Result>("paProductoListar", p);
        }

        public virtual ObjectResult<paVentaListar_Result> paVentaListar(string parametro)
        {
            var p = parametro != null ? new ObjectParameter("parametro", parametro) : new ObjectParameter("parametro", typeof(string));
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<paVentaListar_Result>("paVentaListar", p);
        }

        public virtual ObjectResult<paDetalleVentaListar_Result> paDetalleVentaListar(int idVenta)
        {
            var p = new ObjectParameter("idVenta", idVenta);
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction<paDetalleVentaListar_Result>("paDetalleVentaListar", p);
        }
    }
}
