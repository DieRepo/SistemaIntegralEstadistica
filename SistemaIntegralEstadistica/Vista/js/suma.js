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
        document.getElementById(nombreClase + "_Total").innerHTML = totalPorcentajeSimulacion;
        //document.getElementById('totalPorcentaje' + n).innerHTML = totalPorcentajeSimulacion;

    }catch (Exception ) {
        console.log("ERROR:");
    }

  

}