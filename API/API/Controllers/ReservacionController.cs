using Microsoft.AspNetCore.Mvc;
using API.Models; //referencia para usar los Models
using API.Repository;
using Microsoft.AspNetCore.Authorization; //referencia para usar el dbcontext

namespace API.Controllers
{
    [ApiController]
    [Route("api/Reservaciones")]//Se crea la ruta para los endpoint
    [Authorize]
    public class ReservacionesController : ControllerBase
    {
        //Variable para manejar la referencia del ORM
        private DbContextBeach dbContext = null;

        // constructor con parametros para el api
        public ReservacionesController(DbContextBeach context)
        {
            //se asigna la referencia de dbContext
            this.dbContext = context;
        }

        //Metodo encargado de obtener la lista de reservaciones
        [HttpGet]
        [Route("List")]

        public List<Reservacion> List()
        {
            return this.dbContext.Reservaciones.ToList();
        }

        //Metodo encargado de buscar una reservacion por su id
        [HttpGet]
        [Route("Search")]

        public Reservacion Search(int id)
        {
            Reservacion temp = new Reservacion() { IdReservacion = id };

            try
            {
                //Se busca la reservacion
                var aux = this.dbContext.Reservaciones.FirstOrDefault(x => x.IdReservacion == id);

                //Se valida si existe la reservacion
                if (aux != null)
                {
                    temp = aux;
                }

            }
            catch (Exception ex)
            {
                //Se almacena la informacion en caso de error
                temp.MetodoPago = $"Error, {ex.InnerException.Message}";
            }
            return temp;
        }

        //Metodo encargado de crear una reservacion
        [HttpPost]
        [Route("Create")]

        public String Create(Reservacion temp)
        {
            String msj = "";

            try
            {
                //Se valida si existen datos
                if (temp == null)
                {
                    msj = "No se permiten datos vacios...";
                }
                else if (temp.CantidadNoches <= 0)
                {
                    msj = "La cantidad de noches debe ser mayor que cero...";
                }
                else if (temp.CantidadPersonas <= 0)
                {
                    msj = "La cantidad de personas debe ser mayor que cero...";
                }
                else if (temp.MetodoPago == "")
                {
                    msj = "Debe indicar el metodo de pago...";
                }
                else if (temp.MetodoPago == "Cheque" && temp.NumeroCheque == "")
                {
                    msj = "Debe indicar el numero de cheque...";
                }
                else if (temp.MetodoPago == "Cheque" && temp.BancoCheque == "")
                {
                    msj = "Debe indicar el banco del cheque...";
                }
                else
                {
                    //Se busca el cliente asociado a la reservacion
                    var cliente = this.dbContext.Clientes.FirstOrDefault(x => x.Cedula == temp.Cedula);

                    //Se busca el paquete asociado a la reservacion
                    var paquete = this.dbContext.Paquetes.FirstOrDefault(x => x.IdPaquete == temp.IdPaquete);

                    //Se valida si existe el cliente
                    if (cliente != null)
                    {
                        //Se valida si existe el paquete
                        if (paquete != null)
                        {
                            //Se calcula subtotal
                            temp.SubTotal = paquete.PrecioPorNoche * temp.CantidadNoches;

                            //Se calcula el descuento segun la cantidad de noches y metodo de pago
                            if (temp.MetodoPago == "Efectivo")
                            {
                                if (temp.CantidadNoches >= 3 && temp.CantidadNoches <= 6)
                                {
                                    temp.Descuento = temp.SubTotal * Convert.ToDecimal(0.10);
                                }
                                else if (temp.CantidadNoches >= 7 && temp.CantidadNoches <= 9)
                                {
                                    temp.Descuento = temp.SubTotal * Convert.ToDecimal(0.15);
                                }
                                else if (temp.CantidadNoches >= 10 && temp.CantidadNoches <= 12)
                                {
                                    temp.Descuento = temp.SubTotal * Convert.ToDecimal(0.20);
                                }
                                else if (temp.CantidadNoches >= 13)
                                {
                                    temp.Descuento = temp.SubTotal * Convert.ToDecimal(0.25);
                                }
                                else
                                {
                                    temp.Descuento = 0;
                                }
                            }
                            else
                            {
                                //Tarjeta y cheque no tienen descuento
                                temp.Descuento = 0;
                            }

                            //Se calcula el IVA
                            temp.IVA = temp.SubTotal * Convert.ToDecimal(0.13);

                            //Se calcula el total final
                            temp.TotalFinal = temp.SubTotal + temp.IVA - temp.Descuento;

                            //Se obtiene la prima del paquete
                            temp.Prima = paquete.Prima;

                            //Se calcula mensualidad
                            temp.Mensualidad = (temp.TotalFinal - (temp.TotalFinal * paquete.Prima)) / paquete.Meses;

                            //Se obtiene el tipo de cambio
                            temp.TipoCambio = Convert.ToDecimal(452.22); //Se usa 454.22 ya que es el tipo de cambio actual del dolar según el banco central

                            //Se calcula el total en dolares
                            temp.TotalDolares = temp.TotalFinal / temp.TipoCambio;

                            //Se agrega la fecha actual
                            temp.FechaReservacion = DateTime.Now;

                            //Se agrega la reservacion
                            this.dbContext.Reservaciones.Add(temp);

                            //Se aplican los cambios
                            this.dbContext.SaveChanges();

                            msj = $"Reservacion {temp.IdReservacion} almacenada correctamente...";
                        }
                        else
                        {
                            msj = "El paquete asociado no existe...";
                        }
                    }
                    else
                    {
                        msj = "El cliente asociado no existe...";
                    }
                }

            }
            catch (Exception ex)
            {
                //Se almacena la informacion en caso de error
                msj = $"Error al guardar, {ex.InnerException.Message}";
            }
            return msj;
        }

        //Metodo encargado de actualizar una reservacion
        [HttpPut]
        [Route("Update")]

        public String Update(Reservacion temp)
        {
            String msj = "";

            try
            {
                //Se valida si existen datos
                if (temp == null)
                {
                    msj = "No se permiten datos vacios...";
                }
                else if (temp.CantidadNoches <= 0)
                {
                    msj = "La cantidad de noches debe ser mayor que cero...";
                }
                else if (temp.CantidadPersonas <= 0)
                {
                    msj = "La cantidad de personas debe ser mayor que cero...";
                }
                else if (temp.MetodoPago == "")
                {
                    msj = "Debe indicar el metodo de pago...";
                }
                else if (temp.MetodoPago == "Cheque" && temp.NumeroCheque == "")
                {
                    msj = "Debe indicar el numero de cheque...";
                }
                else if (temp.MetodoPago == "Cheque" && temp.BancoCheque == "")
                {
                    msj = "Debe indicar el banco del cheque...";
                }
                else
                {
                    //Se busca el cliente asociado a la reservacion
                    var cliente = this.dbContext.Clientes.FirstOrDefault(x => x.Cedula == temp.Cedula);

                    //Se busca el paquete asociado a la reservacion
                    var paquete = this.dbContext.Paquetes.FirstOrDefault(x => x.IdPaquete == temp.IdPaquete);

                    //Se valida si existe el cliente
                    if (cliente != null)
                    {
                        //Se valida si existe el paquete
                        if (paquete != null)
                        {
                            //Se busca la reservacion actual
                            var reservacionActual = this.dbContext.Reservaciones.FirstOrDefault(x => x.IdReservacion == temp.IdReservacion);

                            //Se valida si existe la reservacion
                            if (reservacionActual != null)
                            {
                                //Se actualizan los datos
                                reservacionActual.Cedula = temp.Cedula;
                                reservacionActual.IdPaquete = temp.IdPaquete;
                                reservacionActual.CantidadNoches = temp.CantidadNoches;
                                reservacionActual.CantidadPersonas = temp.CantidadPersonas;
                                reservacionActual.MetodoPago = temp.MetodoPago;
                                reservacionActual.NumeroCheque = temp.NumeroCheque;
                                reservacionActual.BancoCheque = temp.BancoCheque;

                                //Se recalcula el subtotal
                                reservacionActual.SubTotal = paquete.PrecioPorNoche * temp.CantidadNoches;

                                //Se recalcula el descuento segun la cantidad de noches y metodo de pago
                                if (temp.MetodoPago == "Efectivo")
                                {
                                    if (temp.CantidadNoches >= 3 && temp.CantidadNoches <= 6)
                                    {
                                        reservacionActual.Descuento = reservacionActual.SubTotal * Convert.ToDecimal(0.10);
                                    }
                                    else if (temp.CantidadNoches >= 7 && temp.CantidadNoches <= 9)
                                    {
                                        reservacionActual.Descuento = reservacionActual.SubTotal * Convert.ToDecimal(0.15);
                                    }
                                    else if (temp.CantidadNoches >= 10 && temp.CantidadNoches <= 12)
                                    {
                                        reservacionActual.Descuento = reservacionActual.SubTotal * Convert.ToDecimal(0.20);
                                    }
                                    else if (temp.CantidadNoches >= 13)
                                    {
                                        reservacionActual.Descuento = reservacionActual.SubTotal * Convert.ToDecimal(0.25);
                                    }
                                    else
                                    {
                                        reservacionActual.Descuento = 0;
                                    }
                                }
                                else
                                {
                                    //Tarjeta y cheque no tienen descuento
                                    reservacionActual.Descuento = 0;
                                }

                                //Se recalcula el IVA
                                reservacionActual.IVA = reservacionActual.SubTotal * Convert.ToDecimal(0.13);

                                //Se recalcula el monto total final
                                reservacionActual.TotalFinal = reservacionActual.SubTotal + reservacionActual.IVA - reservacionActual.Descuento;

                                //Se obtiene prima
                                reservacionActual.Prima = paquete.Prima;

                                //Se recalcula la mensualidad
                                reservacionActual.Mensualidad = (reservacionActual.TotalFinal - (reservacionActual.TotalFinal * paquete.Prima)) / paquete.Meses;

                                //Se obtiene el tipo de cambio 
                                reservacionActual.TipoCambio = Convert.ToDecimal(454.22); //Se usa 454.22 ya que es el tipo de cambio actual del dolar según el banco central

                                //Se recalcula el total en dolares
                                reservacionActual.TotalDolares = reservacionActual.TotalFinal / reservacionActual.TipoCambio;

                                //Se actualiza el registro
                                this.dbContext.Reservaciones.Update(reservacionActual);

                                //Se aplican los cambios
                                this.dbContext.SaveChanges();

                                msj = $"Reservacion {temp.IdReservacion} actualizada correctamente...";
                            }
                            else
                            {
                                msj = $"No existe una reservacion con el id {temp.IdReservacion}";
                            }
                        }
                        else
                        {
                            msj = "El paquete asociado no existe...";
                        }
                    }
                    else
                    {
                        msj = "El cliente asociado no existe...";
                    }
                }

            }
            catch (Exception ex)
            {
                //Se almacena la informacion en caso de error
                msj = $"Error al modificar, {ex.InnerException.Message}";
            }
            return msj;
        }

        //Metodo encargado de eliminar una reservacion por su id
        [HttpDelete]
        [Route("Delete")]

        public String Delete(int id)
        {
            String msj = "";

            try
            {
                //Se busca la reservacion por su id
                var temp = this.dbContext.Reservaciones.FirstOrDefault(x => x.IdReservacion == id);

                //Se valida si existe la reservacion
                if (temp != null)
                {
                    //Se elimina la reservacion
                    this.dbContext.Reservaciones.Remove(temp);

                    //Se aplican los cambios
                    this.dbContext.SaveChanges();

                    msj = "Cambios aplicados correctamente...";
                }
                else
                {
                    msj = $"No existe una reservacion con el id {id}";
                }

            }
            catch (Exception ex)
            {
                //Se almacena la informacion en caso de error
                msj = $"Error al eliminar, {ex.InnerException.Message}";
            }
            return msj;
        }

    }
}