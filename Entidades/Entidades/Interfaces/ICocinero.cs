﻿using Entidades.Enumerables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Interfaces
{
    public interface ICocinero
    {
        IConsumible CrearPlato(string nombreDelPlato, List<IConsumible> ingredientes);
        IConsumible EditarPlato(IConsumible plato, List<IConsumible> ingredientesActualizacion);
        void EliminarPlato(string nombre, List<IConsumible> listaDePlatos);

    }
}
