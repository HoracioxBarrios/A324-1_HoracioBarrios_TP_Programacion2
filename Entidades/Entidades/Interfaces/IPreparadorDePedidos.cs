﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Interfaces
{
    public interface IPreparadorDePedidos
    {
        void TomarPedido(IPedido pedido);
        Task<bool> PrepararPedido();
    }
}
