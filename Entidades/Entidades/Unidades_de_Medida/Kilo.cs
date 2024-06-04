﻿using Entidades.Excepciones;
using Entidades.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entidades.Unidades_de_Medida
{
    public class Kilo : IUnidadDeMedida
    {
        private double _cantidad;

        public Kilo(double cantidad)
        {
            Cantidad = cantidad;
        }

        /// <summary>
        /// Operador de Conversion Explicito de Kilos a Gramos
        /// </summary>
        /// <param name="kilo">recibe un obj. Kilo</param>
        /// <exception cref="AlConvertirException" ></exception>

        public static explicit operator Gramo(Kilo kilo)
        {
            if (kilo.Cantidad > 0)
            {
                double cantidadConvertida = kilo.Cantidad * 1000;
                return new Gramo(cantidadConvertida);
            }
            throw new AlConvertirException("Error al querer Convertir de Kilo a Gramos");
        }



        public static Kilo operator +(Kilo kilo1, Kilo kilo2)
        {
            double nuevaCantidad = kilo1.Cantidad + kilo2.Cantidad;
            if(nuevaCantidad < 0)
            {
                throw new AlSumarException("Error al querer sumar, el resultado da negativo");
            }
            return new Kilo(nuevaCantidad);
        }


        public static Kilo operator -(Kilo kilo1, Kilo kilo2)
        {
            double nuevaCantidad = kilo1.Cantidad - kilo2.Cantidad;
            if(nuevaCantidad < 0)
            {
                throw new AlRestarException("La resta da resultado negativo en KIlos");
            }
            return new Kilo(nuevaCantidad);
        }


        public static Kilo operator +(Kilo kilo, Gramo gramo)
        {
            Kilo nuevoKilo = (Kilo)gramo;
            double nuevaCantidad = kilo.Cantidad + nuevoKilo.Cantidad;
            if (nuevaCantidad < 0)
            {
                throw new AlSumarException("Error al querer sumar, el resultado da negativo ");
            }
            return new Kilo(nuevaCantidad);
        }
        public static Kilo operator -(Kilo kilo, Gramo gramo)
        {
            Kilo nuevoKilo = (Kilo)gramo;
            double nuevaCantidad = kilo.Cantidad - nuevoKilo.Cantidad;
            if( nuevaCantidad < 0)
            {
                throw new AlConvertirException("La resta da resultado negativo de Kilos.");
            }
            return new Kilo(nuevaCantidad);
        }

        public double Cantidad
        {
            get { return _cantidad; }
            set { _cantidad = value; }
        }

    }
}
