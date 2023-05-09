
function sumarTotales(nombreClase) {
    var totalPorcentajeSimulacion = 0;
    try
    {
        $("." + nombreClase).each(function () {
            if (isNaN(parseInt($(this).val()))) {
                totalPorcentajeSimulacion += 0;
            } else {
                totalPorcentajeSimulacion += parseFloat($(this).val());
            }
        });
        var nombreTotal =   nombreClase +  "_Total";
        //document.getElementById(idTotal).innerHTML = totalPorcentajeSimulacion;
        //document.getElementById(idTotal).value = totalPorcentajeSimulacion;
        document.getElementsByClassName(nombreTotal)[0].value = totalPorcentajeSimulacion;
        //document.getElementById('totalPorcentaje' + n).innerHTML = totalPorcentajeSimulacion;
        //console.log(""+totalPorcentajeSimulacion);
        if (nombreClase.startsWith("Tabla1")) {
            sumarTotalMensual(nombreClase.substring(0, nombreClase.length - 2));
        }
    }catch (error ) {
        //console.log("ERROR:"+error.message);
    }

}


function sumarTotalesGeneral(clases, id) {
    var granTotal = 0;

    try {
        var arrayClases = clases.split(",");
        for (var i = 0; i < arrayClases.length ; i++) {
            $("." + arrayClases[i]).each(function () {
                if (isNaN(parseInt($(this).val()))) {
                    granTotal += 0;
                } else {
                    granTotal += parseFloat($(this).val());
                }
            });
        }
        
        var nombreTotal = id;
        document.getElementsByClassName(nombreTotal)[0].value = granTotal;
        
    } catch (error) {
        Console.log("ERROR:"+error.message);
    }

}


function sumarTotalMensual(nombreClase) {
    var totalPorcentaje1 = 0 , totalPorcentaje2=0;
    try {
        $("."+nombreClase+"_1").each(function () {
            if (isNaN(parseInt($(this).val()))) {
                totalPorcentaje1 += 0;
            } else {
                totalPorcentaje1 += parseFloat($(this).val());
            }
        });

        $("." + nombreClase + "_2").each(function () {
            if (isNaN(parseInt($(this).val()))) {
                totalPorcentaje2 += 0;
            } else {
                totalPorcentaje2 += parseFloat($(this).val());
            }
        });
        var nombreTotal = nombreClase + "_2_Total_Mensual";

        document.getElementsByClassName(nombreTotal)[0].value = totalPorcentaje1 + totalPorcentaje2;
        //console.log(""+totalPorcentajeSimulacion);
    } catch (error) {
        //console.log("ERROR:"+error.message);
    }

}


function actualizarTotales(nombreClase , nombreTabla) {
    const fecha = new Date();
    var mes = fecha.getMonth();
    var totalPorcentajeSimulacion = 0;
    try {
        $("." + nombreClase).each(function () {
            if (isNaN(parseInt($(this).val()))) {
                totalPorcentajeSimulacion += 0;
            } else {
                totalPorcentajeSimulacion += parseFloat($(this).val());
            }
        });
        var nombreTotal = nombreClase + "_Total";
        //document.getElementById(idTotal).innerHTML = totalPorcentajeSimulacion;
        //document.getElementById(idTotal).value = totalPorcentajeSimulacion;
        document.getElementsByClassName(nombreTotal)[0].value = totalPorcentajeSimulacion;
        //document.getElementById('totalPorcentaje' + n).innerHTML = totalPorcentajeSimulacion;
        console.log("" + totalPorcentajeSimulacion);
    } catch (error) {
        console.log("ERROR:" + error.message);
    }

}