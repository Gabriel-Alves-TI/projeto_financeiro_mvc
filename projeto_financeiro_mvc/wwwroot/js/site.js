setTimeout(()=> {
    $(".alert").fadeOut("slow", function() {
        $(this).alert("close");
    })
}, 3500)

document.addEventListener("DOMContentLoaded", () => {
    const btnLimpar = document.getElementById('btnLimpar');

    if (!btnLimpar) return; // <-- se não existe, sai fora

    const filtros = document.querySelectorAll('#tipo, #categoria, #conta, #dataInicial, #dataFinal, #valorFiltro');

    btnLimpar.addEventListener('click', () => {
        const dataInicialDefault = btnLimpar.dataset.datainicial;
        const dataFinalDefault = btnLimpar.dataset.datafinal;

        filtros.forEach(input => {
            if (input.id === "dataInicial") {
                input.value = dataInicialDefault;
            } else if (input.id === "dataFinal") {
                input.value = dataFinalDefault;
            } else {
                input.value = "";
            }
        });
    });
});

const formatarMoeda = (value) => {
    value = value.replace(/\D/g, '');
    value = (value / 100).toFixed(2) + '';
    value = value.replace(".", ",");
    value = value.replace(/(\d)(?=(\d{3})+(?!\d))/g, "$1.");
    return value;
}

const formatarInput = (event) => {
    const input = event.target;
    input.value = formatarMoeda(input.value);
}

const inputs = document.querySelectorAll("#valorLancamento, #valorRecorrente, #valorTransferencia, #saldoInicial");
inputs.forEach(input => {
    input.addEventListener('input', formatarInput);
});


document.addEventListener("DOMContentLoaded", function () {
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl)
    })
});