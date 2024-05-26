﻿using System;
using System.Collections.Generic;
using Entidades.Enumerables;
using Entidades.Interfaces;

namespace Entidades
{
    public class Cocinero : Empleado, IEmpleado,  ICocinero
    {
        private ERol Rol {  get; set; }
        private Cocinero() 
        {
            this.Rol = ERol.Cocinero;
        }
        public Cocinero(string nombre, string apellido, string contacto, string direccion, decimal salario) : this() 
        {
            this.Nombre = nombre;
            this.Apellido = apellido;
            this.Contacto = contacto;
            this.Direccion = direccion;
            this.Salario = salario;            
        }


        /// <summary>
        /// Verifica si el plato esta en la lista de Menu
        /// </summary>
        /// <param name="nombrePlato"></param>
        /// <returns>Devuelve si existe en la Lista de Platos </returns>
        private bool ExitePlatoEnLista(string nombrePlato, List<Plato> listPlatosEnMenu)
        {
            bool seEncontro = false;
            foreach (Plato plato in listPlatosEnMenu)
            {
                if (plato.Nombre == nombrePlato)
                {
                    seEncontro = true;
                    break;
                }
            }
            return seEncontro;
        }

        /// <summary>
        /// Crea un Plato (Comida) si no existe aun y lo agrega a listaMenu
        /// </summary>
        /// <param name="nombrePlato"></param>
        /// <param name="listPlatosEnMenu"></param>
        public void CrearPlato(string nombrePlato,List<Plato> listPlatosEnMenu)
        {
            if (ExitePlatoEnLista(nombrePlato, listPlatosEnMenu))
            {
                //Plato nuevoPlato = new Plato(nombrePlato, ingredientes)
                //listPlatosEnMenu.Add(nuevoPlato);
                
            }

            

        }



        public Plato EditarPlato()
        {
            throw new NotImplementedException();
        }

        public Plato EliminarPlato()
        {
            throw new NotImplementedException();
        }

        public Plato CrearPlato()
        {
            throw new NotImplementedException();
        }

    }
}
