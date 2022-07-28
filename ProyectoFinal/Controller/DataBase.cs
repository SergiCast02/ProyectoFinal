using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ProyectoFinal.Models;
using SQLite;

namespace ProyectoFinal.Controller
{
    public class DataBase
    {
        readonly SQLiteAsyncConnection dbase;
        /* Constructor de clase */
        public DataBase(string dbpath)
        {
            dbase = new SQLiteAsyncConnection(dbpath);

            // Crearemos las tablas de la base de datos
            dbase.CreateTableAsync<Usuario>(); // Creando la tabla de Usuarios
            dbase.CreateTableAsync<Persistencia>(); // Creando la tabla de Persistencia
        }

        #region Usuario
        // CRUD - Create - Read - Update - Delete
        // Create
        public Task<int> UsuarioSave(Usuario usuario)
        {
            if (usuario.Id != 0)
            {
                return dbase.UpdateAsync(usuario); // Update
            }
            else
            {
                return dbase.InsertAsync(usuario);
            }
        }

        // Read
        public Task<List<Usuario>> obtenerListaUsuario()
        {
            return dbase.Table<Usuario>().ToListAsync();
        }

        // Read un registro
        public Task<Usuario> obtenerUsuario(int operacion, string dato)
        {
            //1 Obtener usuario por ID
            //2 Obtener usuario por nombre de usuario
            //3 Obtener usuario por correo electrónico

            

            switch (operacion)
            {
                case 1:
                    int usuarioid = int.Parse(dato);
                    return dbase.Table<Usuario>()
                    .Where(i => i.Id == usuarioid)
                    .FirstOrDefaultAsync();
                case 2:
                    return dbase.Table<Usuario>()
                    .Where(i => i.NombreUsuario == dato)
                    .FirstOrDefaultAsync();
                case 3:
                    return dbase.Table<Usuario>()
                    .Where(i => i.Email == dato)
                    .FirstOrDefaultAsync();
            }

            return null;
        }

        // Delete
        public Task<int> UsuarioDelete(Usuario usuario)
        {
            return dbase.DeleteAsync(usuario);
        }

        //Delete ALL
        public Task<int> UsuarioDeleteAll()
        {
            return dbase.DropTableAsync<Usuario>();
        }

        #endregion



        #region Persistencia
        public async Task<int> PersistenciaSave(Persistencia persistencia)
        {
            var registro = await obtenerPersistencia(persistencia.Id);

            if (registro != null) {  return await dbase.UpdateAsync(persistencia); }

            return await dbase.InsertAsync(persistencia);
        }

        // Read
        public Task<List<Persistencia>> obtenerListaPersistencia()
        {
            return dbase.Table<Persistencia>().ToListAsync();
        }

        // Read V2
        public Task<Persistencia> obtenerPersistencia(int pid)
        {
            return dbase.Table<Persistencia>()
                .Where(i => i.Id == pid)
                .FirstOrDefaultAsync();
        }

        // Delete
        public Task<int> PersistenciaDelete(Persistencia persistencia)
        {
            return dbase.DeleteAsync(persistencia);
        }

        //Delete ALL
        public Task<int> PersistenciaDeleteAll()
        {
            return dbase.DropTableAsync<Persistencia>();
        }
        #endregion
    }
}