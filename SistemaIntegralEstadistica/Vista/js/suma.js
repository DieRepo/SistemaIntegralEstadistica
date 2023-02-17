function sumarTotales(nombreClase, continuo) {
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
        var idTotal = "ContentPlaceHolder1_" + nombreClase+"_"+continuo + "_Total";
        //document.getElementById(idTotal).innerHTML = totalPorcentajeSimulacion;
        document.getElementById(idTotal).value = totalPorcentajeSimulacion;
        //document.getElementById('totalPorcentaje' + n).innerHTML = totalPorcentajeSimulacion;
        console.log(""+totalPorcentajeSimulacion);
    }catch (error ) {
        console.log("ERROR:"+error.message);
    }

  

}