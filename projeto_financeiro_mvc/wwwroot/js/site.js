setTimeout(()=> {
    $(".alert").fadeOut("slow", () => {
        $(this).alert("close");
    })
}, 3500)

const btnLimpar = document.getElementById('btnLimpar');
const filtros = document.querySelectorAll('#tipo, #categoria, #conta, #dataInicial, #dataFinal');

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
})