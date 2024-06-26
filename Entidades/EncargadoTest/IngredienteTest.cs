﻿using Entidades;
using Entidades.Enumerables;
using Entidades.Interfaces;
using Entidades.Services;
using Moq;
using Negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    [TestClass]
    public class IngredienteTest
    {
        [TestMethod]
        public async Task DescontarIngredienteDelStock_DebeDescontarUnIngredienteDeUnaListaDeProductosQueHayEnStock_SiSeDescontóDaTrue()
        {

            //Arrange
            //DATOS PARA Ingrediente 1-----------------------------------
            ETipoDeProducto tipoDeProducto1 = ETipoDeProducto.Ingrediente;
            string nombreDeProducto1 = "Pollo";
            double cantidad = 20;
            EUnidadDeMedida unidadDeMedida = EUnidadDeMedida.Kilo;
            decimal precio = 20000;

            var mockProveedor1 = new Mock<IProveedor>();
            mockProveedor1.Setup(p => p.Nombre).Returns("Proveedor 1");
            mockProveedor1.Setup(p => p.Cuit).Returns("30-12345678-9");
            mockProveedor1.Setup(p => p.Direccion).Returns("Calle Falsa 123");
            mockProveedor1.Setup(p => p.TipoDeProducto).Returns(ETipoDeProducto.Almacen);
            mockProveedor1.Setup(p => p.MediosDePago).Returns(EMediosDePago.Transferencia);
            mockProveedor1.Setup(p => p.EsAcreedor).Returns(EAcreedor.Si);
            mockProveedor1.Setup(p => p.DiaDeEntrega).Returns(EDiaDeLaSemana.Lunes);
            mockProveedor1.Setup(p => p.ID).Returns(1);
            mockProveedor1.Setup(p => p.ToString()).Returns("ID: 1, Nombre: Proveedor 1, CUIT: 30-12345678-9, Direccion: Calle Falsa 123, Tipo de Producto que Provee: Almacen, Medio de Pago: Transferencia, Es Acreedor? : Si, Dia de Entrega: Lunes");

            //DATOS PARA Ingrediente 2 --------------------------------
            ETipoDeProducto tipoDeProducto2 = ETipoDeProducto.Ingrediente;
            string nombreDeProducto2 = "Papa";
            double cantidad2 = 20;
            EUnidadDeMedida unidadDeMedida2 = EUnidadDeMedida.Kilo;
            decimal precio2 = 20000;

            var mockProveedor2 = new Mock<IProveedor>();
            mockProveedor2.Setup(p => p.Nombre).Returns("Proveedor 2");
            mockProveedor2.Setup(p => p.Cuit).Returns("31-12345678-8");
            mockProveedor2.Setup(p => p.Direccion).Returns("Calle Falsa 456");
            mockProveedor2.Setup(p => p.TipoDeProducto).Returns(ETipoDeProducto.Carniceria);
            mockProveedor2.Setup(p => p.MediosDePago).Returns(EMediosDePago.Tarjeta);
            mockProveedor2.Setup(p => p.EsAcreedor).Returns(EAcreedor.No);
            mockProveedor2.Setup(p => p.DiaDeEntrega).Returns(EDiaDeLaSemana.Martes);
            mockProveedor2.Setup(p => p.ID).Returns(2);
            mockProveedor2.Setup(p => p.ToString()).Returns("ID: 2, Nombre: Proveedor 2, CUIT: 31-12345678-8, Direccion: Calle Falsa 456, Tipo de Producto que Provee: Carniceria, Medio de Pago: Tarjeta, Es Acreedor? : No, Dia de Entrega: Martes");




            //------------------- GESTOR DE PRODUCTOS -----------------------
            GestorDeProductos gestorDeProductos = new GestorDeProductos();


            //Act
            //Productos que van a estar en la lista del stock (está dentro de GestorProductos)

            // -------------- CREAMOS LOS INGREDIENTES para el stock ------------------
            IProducto pollo = gestorDeProductos.CrearProducto(tipoDeProducto1, nombreDeProducto1, cantidad, unidadDeMedida,precio, mockProveedor1.Object);
            IProducto papa = gestorDeProductos.CrearProducto(tipoDeProducto2, nombreDeProducto2, cantidad2, unidadDeMedida2, precio2, mockProveedor2.Object);

            //AGREGAMOS AL STOCK
            gestorDeProductos.AgregarProductoAStock(pollo);
            gestorDeProductos.AgregarProductoAStock(papa);

            //Instanciamos EL COCINERO
            IEmpleado cocinero = EmpleadoServiceFactory.CrearEmpleado(ERol.Cocinero, "Pipo", "Sdd", "2323", "Av pepe 123", 15000);


            //INSTANCIAMOS EL GESTOR MENU
            GestorDeMenu gestormenu = new GestorDeMenu((ICocinero)cocinero, gestorDeProductos);


            //SELECCIONAMOS INGREDIENTES PARA EL PLATO (DEBE ESTAR CREADO EN SISTEMA ( -- STOCK --)) -- para el PLATO deben ser 
            gestormenu.SelecionarIngrediente("Pollo", 1, EUnidadDeMedida.Kilo);
            gestormenu.SelecionarIngrediente("Papa", 1, EUnidadDeMedida.Kilo);

            // ----------------- Creamos el Menu -------------------
            gestormenu.CrearMenu("Almuerzo");

            // -------------- Creamos el plato ---------------------
            string nombrePlato = "polloPapa";
            int tiempoPreparacion = 10;
            EUnidadDeTiempo unidadTiempo = EUnidadDeTiempo.Segundos;

            IConsumible plato = gestormenu.CrearPlato(nombrePlato, tiempoPreparacion,unidadTiempo);

            //Agregamos el plato al menu
            gestormenu.AgregarPlatoAMenu("Almuerzo", plato);




            // >>>>>>>>>>>>>>>>>> ---- Traemos los menus disponibles Mostramos el menu ---- <<<<<<<<<<<<<<<<<<<<<<<<<<
            List<IMenu> menusDisponibles = gestormenu.GetAllMenus();
            IMenu menuSeleccionado = gestormenu.GetMenuPorNombre("Almuerzo");//selecionamos un menu

            //----------- SELECCION DE CONSUMIBLES ( son los que elijen los comensales o clientes) con esto Armaremos el Pedido( solo sera selecionable la bebidas que hayan en stock y los platos que se puedan cocinar) --------------------
            

            List<IConsumible> consumublesSelecionadosParaPedido = new List<IConsumible>(); // listo para los consumibles del pedido

            IConsumible plato1 = menuSeleccionado.GetPlatoPorNombre("polloPapa");//del menu traemos el plato elegido por el cliente


            consumublesSelecionadosParaPedido.Add(plato1);
            //---------------------- CREAMOS EL PEDIDO ------------------------

            GestorDePedidos gestorDePedidos = new GestorDePedidos();

            //ICreador de Pedidos MESERO O ENCARGADO
            //EN CASO DEL MESERO DEBE ESTAR ASIGNADO A LA MESA:
            IEmpleado encargado = EmpleadoServiceFactory.CrearEmpleado(ERol.Encargado, "Frey", "Varga","421544", "Av. los copos 66", 45000M);

            GestorDeMesas gestorMesas = new GestorDeMesas((IEncargado)encargado, 4);

            

            IEmpleado mesero = EmpleadoServiceFactory.CrearEmpleado(ERol.Mesero, "Leo", "Gry","1152000" , "Av iglu 45", 15000M);

            gestorMesas.RegistrarMesero((IMesero)mesero); //Registro el Mesero en el gestor mesas

            gestorMesas.AsignarMesaAMesero("Leo", "Gry", 1);


            //Id de la mesa que realiza el pedido
            int idDeLaMesCliente = 1;

            bool seCreoPedido = gestorDePedidos.CrearPedido((ICreadorDePedidos)mesero ,ETipoDePedido.Para_Local, consumublesSelecionadosParaPedido, idDeLaMesCliente);

            //------------------------------------------------------ cuando SE CREa EL PEDIDO ya lo tenemos disponible
            //DEBEMOS TOMAR ESE PEDIDO O COMANDA :por eemplo el cocinero que va a preparar los platos del pedido

            IPedido pedido = gestorDePedidos.TomarPedidoPrioritario();


            //Preparar pedido el cocinero recibe el pedido, los PLATOS TARDAN EN COCINARSE y cuando esten los platos cocinados (el pedido pasara a estar disponible ), LAS BEBIDAS SE TOMAN COMO ENTREGABLES SI ESTAN disponibles EN STOCK
            bool estaListoElPedido = await gestorDePedidos.PrepararPedido((IPreparadorDePedidos) cocinero, pedido);

            //CUANDO TERMINA EL TIEMPO ( de todos los platos )-----> avisa por evento que el pedido esta LISTO PARA ENTREGAR
            if (estaListoElPedido == true)
            {
                Assert.IsTrue(estaListoElPedido);



                gestorDePedidos.EntregarPedido()

                /*
                 
                 2- falta el entregar pedido a la mesa,
                 3- luego cerrar la mesa y el mesero debe reflejar dentro lo cobrado por el pédido entregado.
                 */



            }
            


            //El cocinero ASIGNA EL PLATO A LA MESA y se debe DESCONTAR LOS INGREDIENTES QUE SE USARON EN EL PLATO.




            //Se le pasa la lista de Ingredintes a desconcar
            //bool seDesconto = gestorDeProductos.DescontarProductosDeStock(listaDeIngredienteEnElPlato);

            //------------------------------------------------------------------------------------------------------------------ CORREGIR ACA

            //Assert
            //Si se descuenta
            Assert.IsTrue(seDesconto);



        }

        [TestMethod]
        public void VerElIngredienteEnKilosMenosKilos_DebeDescontarUnIngredienteDeUnaListaDeProductosQueHayEnStock_ALCorroborarDeeQuedar9KilosDEPollo()
        {

            //Arrange
            //Ingrediente 1
            ETipoDeProducto tipoDeProducto1 = ETipoDeProducto.Ingrediente;
            string nombreDeProducto1 = "Pollo";
            double cantidad = 10;
            EUnidadDeMedida unidadDeMedida = EUnidadDeMedida.Kilo;
            decimal precio = 20000;

            var mockProveedor1 = new Mock<IProveedor>();
            mockProveedor1.Setup(p => p.Nombre).Returns("Proveedor 1");
            mockProveedor1.Setup(p => p.Cuit).Returns("30-12345678-9");
            mockProveedor1.Setup(p => p.Direccion).Returns("Calle Falsa 123");
            mockProveedor1.Setup(p => p.TipoDeProducto).Returns(ETipoDeProducto.Almacen);
            mockProveedor1.Setup(p => p.MediosDePago).Returns(EMediosDePago.Transferencia);
            mockProveedor1.Setup(p => p.EsAcreedor).Returns(EAcreedor.Si);
            mockProveedor1.Setup(p => p.DiaDeEntrega).Returns(EDiaDeLaSemana.Lunes);
            mockProveedor1.Setup(p => p.ID).Returns(1);
            mockProveedor1.Setup(p => p.ToString()).Returns("ID: 1, Nombre: Proveedor 1, CUIT: 30-12345678-9, Direccion: Calle Falsa 123, Tipo de Producto que Provee: Almacen, Medio de Pago: Transferencia, Es Acreedor? : Si, Dia de Entrega: Lunes");

            //Ingrediente 2 
            ETipoDeProducto tipoDeProducto2 = ETipoDeProducto.Ingrediente;
            string nombreDeProducto2 = "Papa";
            double cantidad2 = 30;
            EUnidadDeMedida unidadDeMedida2 = EUnidadDeMedida.Kilo;
            decimal precio2 = 20000;

            var mockProveedor2 = new Mock<IProveedor>();
            mockProveedor2.Setup(p => p.Nombre).Returns("Proveedor 2");
            mockProveedor2.Setup(p => p.Cuit).Returns("31-12345678-8");
            mockProveedor2.Setup(p => p.Direccion).Returns("Calle Falsa 456");
            mockProveedor2.Setup(p => p.TipoDeProducto).Returns(ETipoDeProducto.Carniceria);
            mockProveedor2.Setup(p => p.MediosDePago).Returns(EMediosDePago.Tarjeta);
            mockProveedor2.Setup(p => p.EsAcreedor).Returns(EAcreedor.No);
            mockProveedor2.Setup(p => p.DiaDeEntrega).Returns(EDiaDeLaSemana.Martes);
            mockProveedor2.Setup(p => p.ID).Returns(2);
            mockProveedor2.Setup(p => p.ToString()).Returns("ID: 2, Nombre: Proveedor 2, CUIT: 31-12345678-8, Direccion: Calle Falsa 456, Tipo de Producto que Provee: Carniceria, Medio de Pago: Tarjeta, Es Acreedor? : No, Dia de Entrega: Martes");

            //Ingrediente que se instancia en el plato y estaria en la lista del plato.
            //Ingrediente 3
            ETipoDeProducto tipoDeProducto3 = ETipoDeProducto.Ingrediente;
            string nombreDeProducto3 = "Pollo";
            double cantidad3 = 1; // Cantidad que usa en el plato
            EUnidadDeMedida unidadDeMedida3 = EUnidadDeMedida.Kilo;
            decimal precio3 = 1000;

            var mockProveedor3 = new Mock<IProveedor>();
            mockProveedor3.Setup(p => p.Nombre).Returns("Proveedor 1");
            mockProveedor3.Setup(p => p.Cuit).Returns("30-12345678-9");
            mockProveedor3.Setup(p => p.Direccion).Returns("Calle Falsa 123");
            mockProveedor3.Setup(p => p.TipoDeProducto).Returns(ETipoDeProducto.Almacen);
            mockProveedor3.Setup(p => p.MediosDePago).Returns(EMediosDePago.Transferencia);
            mockProveedor3.Setup(p => p.EsAcreedor).Returns(EAcreedor.Si);
            mockProveedor3.Setup(p => p.DiaDeEntrega).Returns(EDiaDeLaSemana.Lunes);
            mockProveedor3.Setup(p => p.ID).Returns(1);
            mockProveedor3.Setup(p => p.ToString()).Returns("ID: 1, Nombre: Proveedor 1, CUIT: 30-12345678-9, Direccion: Calle Falsa 123, Tipo de Producto que Provee: Almacen, Medio de Pago: Transferencia, Es Acreedor? : Si, Dia de Entrega: Lunes");

            GestorDeProductos gestorDeProductos = new GestorDeProductos();


            //Act
            //Productos que van a estar en la lista del stock (está dentro de GestorProductos)
            gestorDeProductos.CrearProductoParaListaDeStock(tipoDeProducto1, nombreDeProducto1, cantidad, unidadDeMedida, precio, mockProveedor1.Object);
            gestorDeProductos.CrearProductoParaListaDeStock(tipoDeProducto2, nombreDeProducto2, cantidad2, unidadDeMedida2, precio2, mockProveedor2.Object);


            //Producto IConsumubleque va a estar en el plato(lo que  usa el plato)
            IProducto ingrediente3 = gestorDeProductos.CrearProducto(tipoDeProducto3, nombreDeProducto3, cantidad3, unidadDeMedida3, precio3, mockProveedor3.Object);

            List<IConsumible> listaDeIngredienteEnElPlato = new List<IConsumible>();
            listaDeIngredienteEnElPlato.Add((IConsumible)ingrediente3);

            //Se le pasa la lista de Ingredintes a descontar
            bool seDesconto = gestorDeProductos.DescontarProductosDeStock(listaDeIngredienteEnElPlato);





            foreach (IProducto productoIngrediente in gestorDeProductos.ReadAllProductos())
            {
                if (productoIngrediente.Nombre == "Pollo" && productoIngrediente.Cantidad == 9)
                {
                    Assert.AreEqual(9, productoIngrediente.Cantidad);
                }
            }
        }

        [TestMethod]
        public void ElIngredienteEnLitrosMenosMiliLitros_DebeDescontarUnIngredienteDeUnaListaDeProductosQueHayEnStock_ALCorroborarDeeQuedar8coma5Litros()
        {

            //Arrange
            //Ingrediente 1
            ETipoDeProducto tipoDeProducto1 = ETipoDeProducto.Ingrediente;
            string nombreDeProducto1 = "Aceite";
            double cantidad = 9;
            EUnidadDeMedida unidadDeMedida = EUnidadDeMedida.Litro;
            decimal precio = 20000;

            var mockProveedor1 = new Mock<IProveedor>();
            mockProveedor1.Setup(p => p.Nombre).Returns("Proveedor 1");
            mockProveedor1.Setup(p => p.Cuit).Returns("30-12345678-9");
            mockProveedor1.Setup(p => p.Direccion).Returns("Calle Falsa 123");
            mockProveedor1.Setup(p => p.TipoDeProducto).Returns(ETipoDeProducto.Almacen);
            mockProveedor1.Setup(p => p.MediosDePago).Returns(EMediosDePago.Transferencia);
            mockProveedor1.Setup(p => p.EsAcreedor).Returns(EAcreedor.Si);
            mockProveedor1.Setup(p => p.DiaDeEntrega).Returns(EDiaDeLaSemana.Lunes);
            mockProveedor1.Setup(p => p.ID).Returns(1);
            mockProveedor1.Setup(p => p.ToString()).Returns("ID: 1, Nombre: Proveedor 1, CUIT: 30-12345678-9, Direccion: Calle Falsa 123, Tipo de Producto que Provee: Almacen, Medio de Pago: Transferencia, Es Acreedor? : Si, Dia de Entrega: Lunes");

            //Ingrediente 2 
            ETipoDeProducto tipoDeProducto2 = ETipoDeProducto.Ingrediente;
            string nombreDeProducto2 = "Carne";
            double cantidad2 = 50;
            EUnidadDeMedida unidadDeMedida2 = EUnidadDeMedida.Kilo;
            decimal precio2 = 20000;

            var mockProveedor2 = new Mock<IProveedor>();
            mockProveedor2.Setup(p => p.Nombre).Returns("Proveedor 2");
            mockProveedor2.Setup(p => p.Cuit).Returns("31-12345678-8");
            mockProveedor2.Setup(p => p.Direccion).Returns("Calle Falsa 456");
            mockProveedor2.Setup(p => p.TipoDeProducto).Returns(ETipoDeProducto.Carniceria);
            mockProveedor2.Setup(p => p.MediosDePago).Returns(EMediosDePago.Tarjeta);
            mockProveedor2.Setup(p => p.EsAcreedor).Returns(EAcreedor.No);
            mockProveedor2.Setup(p => p.DiaDeEntrega).Returns(EDiaDeLaSemana.Martes);
            mockProveedor2.Setup(p => p.ID).Returns(2);
            mockProveedor2.Setup(p => p.ToString()).Returns("ID: 2, Nombre: Proveedor 2, CUIT: 31-12345678-8, Direccion: Calle Falsa 456, Tipo de Producto que Provee: Carniceria, Medio de Pago: Tarjeta, Es Acreedor? : No, Dia de Entrega: Martes");

            //Ingrediente que se instancia en el plato y estaria en la lista del plato.
            //Ingrediente 3
            ETipoDeProducto tipoDeProducto3 = ETipoDeProducto.Ingrediente;
            string nombreDeProducto3 = "Aceite";
            double cantidad3 = 500; // Cantidad que usa en el plato
            EUnidadDeMedida unidadDeMedida3 = EUnidadDeMedida.MiliLitro;
            decimal precio3 = 1000;

            var mockProveedor3 = new Mock<IProveedor>();
            mockProveedor3.Setup(p => p.Nombre).Returns("Proveedor 1");
            mockProveedor3.Setup(p => p.Cuit).Returns("30-12345678-9");
            mockProveedor3.Setup(p => p.Direccion).Returns("Calle Falsa 123");
            mockProveedor3.Setup(p => p.TipoDeProducto).Returns(ETipoDeProducto.Almacen);
            mockProveedor3.Setup(p => p.MediosDePago).Returns(EMediosDePago.Transferencia);
            mockProveedor3.Setup(p => p.EsAcreedor).Returns(EAcreedor.Si);
            mockProveedor3.Setup(p => p.DiaDeEntrega).Returns(EDiaDeLaSemana.Lunes);
            mockProveedor3.Setup(p => p.ID).Returns(1);
            mockProveedor3.Setup(p => p.ToString()).Returns("ID: 1, Nombre: Proveedor 1, CUIT: 30-12345678-9, Direccion: Calle Falsa 123, Tipo de Producto que Provee: Almacen, Medio de Pago: Transferencia, Es Acreedor? : Si, Dia de Entrega: Lunes");

            GestorDeProductos gestorDeProductos = new GestorDeProductos();


            //Act
            //Productos que van a estar en la lista del stock (está dentro de GestorProductos)
            gestorDeProductos.CrearProductoParaListaDeStock(tipoDeProducto1, nombreDeProducto1, cantidad, unidadDeMedida, precio, mockProveedor1.Object);
            gestorDeProductos.CrearProductoParaListaDeStock(tipoDeProducto2, nombreDeProducto2, cantidad2, unidadDeMedida2, precio2, mockProveedor2.Object);


            //Producto que va a estar en el plato(lo que  usa el plato)
            IProducto ingrediente3 = gestorDeProductos.CrearProducto(tipoDeProducto3, nombreDeProducto3, cantidad3, unidadDeMedida3, precio3, mockProveedor3.Object);

            List<IConsumible> listaDeIngredienteEnElPlato = new List<IConsumible>();
            listaDeIngredienteEnElPlato.Add((IConsumible)ingrediente3);

            //Se le pasa la lista de Ingredintes a descontar
            bool seDesconto = gestorDeProductos.DescontarProductosDeStock(listaDeIngredienteEnElPlato);





            foreach (IProducto productoIngrediente in gestorDeProductos.ReadAllProductos())
            {
                if (productoIngrediente.Nombre == "Aceite" && productoIngrediente.Cantidad == 8.5)
                {
                    Assert.AreEqual(8.5, productoIngrediente.Cantidad);
                }
            }
        }

        [TestMethod]
        public void ElIngredienteQueEstaEnLitrosSeDescuentaEnMiliLitrosYActualizaElValorEnBaseASuCantidad_DebeDescontarElAceiteDeUnaListaDeProductosQueHayEnStock_ALCorroborarDebeQuedar9coma5LitrosYTambienDebeValerTodoNueveMilQuinientosPesos()
        {

            //Arrange
            //Ingrediente 1
            ETipoDeProducto tipoDeProducto1 = ETipoDeProducto.Ingrediente;
            string nombreDeProducto1 = "Aceite";
            double cantidad = 10;
            EUnidadDeMedida unidadDeMedida = EUnidadDeMedida.Litro;
            decimal precio = 10000;

            var mockProveedor1 = new Mock<IProveedor>();
            mockProveedor1.Setup(p => p.Nombre).Returns("Proveedor 1");
            mockProveedor1.Setup(p => p.Cuit).Returns("30-12345678-9");
            mockProveedor1.Setup(p => p.Direccion).Returns("Calle Falsa 123");
            mockProveedor1.Setup(p => p.TipoDeProducto).Returns(ETipoDeProducto.Almacen);
            mockProveedor1.Setup(p => p.MediosDePago).Returns(EMediosDePago.Transferencia);
            mockProveedor1.Setup(p => p.EsAcreedor).Returns(EAcreedor.Si);
            mockProveedor1.Setup(p => p.DiaDeEntrega).Returns(EDiaDeLaSemana.Lunes);
            mockProveedor1.Setup(p => p.ID).Returns(1);
            mockProveedor1.Setup(p => p.ToString()).Returns("ID: 1, Nombre: Proveedor 1, CUIT: 30-12345678-9, Direccion: Calle Falsa 123, Tipo de Producto que Provee: Almacen, Medio de Pago: Transferencia, Es Acreedor? : Si, Dia de Entrega: Lunes");

            //Ingrediente 2 
            ETipoDeProducto tipoDeProducto2 = ETipoDeProducto.Ingrediente;
            string nombreDeProducto2 = "Carne";
            double cantidad2 = 50;
            EUnidadDeMedida unidadDeMedida2 = EUnidadDeMedida.Kilo;
            decimal precio2 = 20000;

            var mockProveedor2 = new Mock<IProveedor>();
            mockProveedor2.Setup(p => p.Nombre).Returns("Proveedor 2");
            mockProveedor2.Setup(p => p.Cuit).Returns("31-12345678-8");
            mockProveedor2.Setup(p => p.Direccion).Returns("Calle Falsa 456");
            mockProveedor2.Setup(p => p.TipoDeProducto).Returns(ETipoDeProducto.Carniceria);
            mockProveedor2.Setup(p => p.MediosDePago).Returns(EMediosDePago.Tarjeta);
            mockProveedor2.Setup(p => p.EsAcreedor).Returns(EAcreedor.No);
            mockProveedor2.Setup(p => p.DiaDeEntrega).Returns(EDiaDeLaSemana.Martes);
            mockProveedor2.Setup(p => p.ID).Returns(2);
            mockProveedor2.Setup(p => p.ToString()).Returns("ID: 2, Nombre: Proveedor 2, CUIT: 31-12345678-8, Direccion: Calle Falsa 456, Tipo de Producto que Provee: Carniceria, Medio de Pago: Tarjeta, Es Acreedor? : No, Dia de Entrega: Martes");

            //Ingrediente que se instancia en el plato y estaria en la lista del plato.
            //Ingrediente 3
            ETipoDeProducto tipoDeProducto3 = ETipoDeProducto.Ingrediente;
            string nombreDeProducto3 = "Aceite";
            double cantidad3 = 500; // Cantidad que usa en el plato
            EUnidadDeMedida unidadDeMedida3 = EUnidadDeMedida.MiliLitro;
            decimal precio3 = 1000;

            var mockProveedor3 = new Mock<IProveedor>();
            mockProveedor3.Setup(p => p.Nombre).Returns("Proveedor 1");
            mockProveedor3.Setup(p => p.Cuit).Returns("30-12345678-9");
            mockProveedor3.Setup(p => p.Direccion).Returns("Calle Falsa 123");
            mockProveedor3.Setup(p => p.TipoDeProducto).Returns(ETipoDeProducto.Almacen);
            mockProveedor3.Setup(p => p.MediosDePago).Returns(EMediosDePago.Transferencia);
            mockProveedor3.Setup(p => p.EsAcreedor).Returns(EAcreedor.Si);
            mockProveedor3.Setup(p => p.DiaDeEntrega).Returns(EDiaDeLaSemana.Lunes);
            mockProveedor3.Setup(p => p.ID).Returns(1);
            mockProveedor3.Setup(p => p.ToString()).Returns("ID: 1, Nombre: Proveedor 1, CUIT: 30-12345678-9, Direccion: Calle Falsa 123, Tipo de Producto que Provee: Almacen, Medio de Pago: Transferencia, Es Acreedor? : Si, Dia de Entrega: Lunes");

            GestorDeProductos gestorDeProductos = new GestorDeProductos();


            //Act
            //Productos que van a estar en la lista del stock (está dentro de GestorProductos)
            gestorDeProductos.CrearProductoParaListaDeStock(tipoDeProducto1, nombreDeProducto1, cantidad, unidadDeMedida, precio, mockProveedor1.Object);
            gestorDeProductos.CrearProductoParaListaDeStock(tipoDeProducto2, nombreDeProducto2, cantidad2, unidadDeMedida2, precio2, mockProveedor2.Object);


            //Producto que va a estar en el plato(lo que  usa el plato)
            IProducto ingrediente3 = gestorDeProductos.CrearProducto(tipoDeProducto3, nombreDeProducto3, cantidad3, unidadDeMedida3, precio3, mockProveedor3.Object);

            List<IConsumible> listaDeIngredienteEnElPlato = new List<IConsumible>();
            listaDeIngredienteEnElPlato.Add((IConsumible)ingrediente3);

            //Se le pasa la lista de Ingredintes a descontar
            bool seDesconto = gestorDeProductos.DescontarProductosDeStock(listaDeIngredienteEnElPlato);





            foreach (IProducto productoIngrediente in gestorDeProductos.ReadAllProductos())
            {
                if (productoIngrediente.Nombre == "Aceite" && productoIngrediente.Cantidad == 8.5)
                {
                    Assert.AreEqual(8.5, productoIngrediente.Cantidad);
                    Assert.AreEqual(9500, productoIngrediente.CalcularPrecioDeCosto());
                }
            }
        }
    }
}
