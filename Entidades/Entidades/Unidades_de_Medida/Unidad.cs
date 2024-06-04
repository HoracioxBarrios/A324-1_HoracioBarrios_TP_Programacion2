﻿using Entidades.Excepciones;
using Entidades.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Unidades_de_Medida
{
    public class Unidad : IUnidadDeMedida
    {
        private double _cantidad;

        public Unidad(double cantidad) 
        { 
            Cantidad = cantidad;
        }

        public static explicit operator Unidad(double valor)
        {
            return new Unidad(valor);
        }

        public static Unidad operator+(Unidad unidad1, Unidad unidad2)
        {
            double nuevaCantidad = unidad1.Cantidad + unidad2.Cantidad;
            if(nuevaCantidad < 0)
            {
                throw new AlSumarException("Error al sumar, el resultado da negativo");
            }
            return new Unidad(nuevaCantidad);
        }
        public static Unidad operator -(Unidad unidad1, Unidad unidad2)
        {
            double nuevaCantidad = unidad1.Cantidad - unidad2.Cantidad;
            if (nuevaCantidad < 0)
            {
                throw new AlSumarException("Error al Restar, el resultado da negativo");
            }
            return new Unidad(nuevaCantidad);
        }

        public double Cantidad 
        {
            get { return _cantidad; }
            set { _cantidad = value; }
        }

    }
}
