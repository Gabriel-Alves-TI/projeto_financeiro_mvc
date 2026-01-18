document.addEventListener("DOMContentLoaded", () => {

    const imgNav = document.getElementById("img-nav");

    if (!imgNav) {
        console.log("img-nav não encontrada");
        return;
    }

    const theme = localStorage.getItem("theme") || "Vapor";
    let viewTitle = document.getElementById('viewTitle').textContent;
    console.log("Theme:", theme);

    if (theme === "Brite") { //Tema Brite
        if (viewTitle.includes("Dashboard"))
        {
            imgNav.src = "/img/brite/dashboard_brite.png";
        }
        if (viewTitle.includes("Lançamento"))
        {
            imgNav.src = "/img/brite/lancamentos_brite.png";
        }
        if (viewTitle.includes("Categoria"))
        {
            imgNav.src = "/img/brite/categorias_brite.png";
        }
        if (viewTitle.includes("Extrato"))
        {
            imgNav.src = "/img/brite/extrato_brite.png";
        }
        if (viewTitle.includes("Conta"))
        {
            imgNav.src = "/img/brite/cofre_brite.png";
        }
        if (viewTitle.includes("Preferências"))
        {
            imgNav.src = "/img/brite/configuracoes_brite.png";
        }
        if (viewTitle.includes("Temas"))
        {
            imgNav.src = "/img/temas.png";
            imgNav.style.scale = "0.7";
        }
    }
    else if (theme === "Cerulean") { //Tema Cerulean
        if (viewTitle.includes("Dashboard"))
        {
            imgNav.src = "/img/dashboard_azul.png";
        }
        if (viewTitle.includes("Lançamento"))
        {
            imgNav.src = "/img/lancamentos_azul.png";
        }
        if (viewTitle.includes("Categoria"))
        {
            imgNav.src = "/img/cerulean/categorias_cerulean.png";
        }
        if (viewTitle.includes("Extrato"))
        {
            imgNav.src = "/img/extrato_azul.png";
        }
        if (viewTitle.includes("Conta"))
        {
            imgNav.src = "/img/cerulean/cofre_cerulean.png";
        }
        if (viewTitle.includes("Preferências"))
        {
            imgNav.src = "/img/cerulean/configuracoes_cerulean.png";
        }
        if (viewTitle.includes("Temas"))
        {
            imgNav.src = "/img/temas.png";
            imgNav.style.scale = "0.7";
        }
    }
    else if (theme === "Cosmo") { //Tema Cosmo
        if (viewTitle.includes("Dashboard"))
        {
            imgNav.src = "/img/dashboard_azul.png";
        }
        if (viewTitle.includes("Lançamento"))
        {
            imgNav.src = "/img/lancamentos_azul.png";
        }
        if (viewTitle.includes("Categoria"))
        {
            imgNav.src = "/img/cosmo/categorias_cosmo.png";
        }
        if (viewTitle.includes("Extrato"))
        {
            imgNav.src = "/img/extrato_azul.png";
        }
        if (viewTitle.includes("Conta"))
        {
            imgNav.src = "/img/cosmo/cofre_cosmo.png";
        }
        if (viewTitle.includes("Preferências"))
        {
            imgNav.src = "/img/cosmo/configuracoes_cosmo.png";
        }
        if (viewTitle.includes("Temas"))
        {
            imgNav.src = "/img/temas.png";
            imgNav.style.scale = "0.7";
        }
    }
    else if (theme === "Cyborg") { //Tema Cyborg
        if (viewTitle.includes("Dashboard"))
        {
            imgNav.src = "/img/dashboard_azul.png";
        }
        if (viewTitle.includes("Lançamento"))
        {
            imgNav.src = "/img/lancamentos_azul.png";
        }
        if (viewTitle.includes("Categoria"))
        {
            imgNav.src = "/img/categorias.png";
        }
        if (viewTitle.includes("Extrato"))
        {
            imgNav.src = "/img/extrato_azul.png";
        }
        if (viewTitle.includes("Conta"))
        {
            imgNav.src = "/img/cofre.png";
        }
        if (viewTitle.includes("Preferências"))
        {
            imgNav.src = "/img/configuracoes_cinza.png";
        }
        if (viewTitle.includes("Temas"))
        {
            imgNav.src = "/img/temas.png";
            imgNav.style.scale = "0.7";
        }
    }
    else if (theme === "Darkly") { //Tema Darkly
        if (viewTitle.includes("Dashboard"))
        {
            imgNav.src = "/img/darkly/dashboard_darkly.png";
        }
        if (viewTitle.includes("Lançamento"))
        {
            imgNav.src = "/img/darkly/lancamento_darkly.png";
        }
        if (viewTitle.includes("Categoria"))
        {
            imgNav.src = "/img/categorias.png";
        }
        if (viewTitle.includes("Extrato"))
        {
            imgNav.src = "/img/darkly/extrato_darkly.png";
        }
        if (viewTitle.includes("Conta"))
        {
            imgNav.src = "/img/cofre.png";
        }
        if (viewTitle.includes("Preferências"))
        {
            imgNav.src = "/img/configuracoes_cinza.png";
        }
        if (viewTitle.includes("Temas"))
        {
            imgNav.src = "/img/temas.png";
            imgNav.style.scale = "0.7";
        }
    }
    else if (theme === "Sketchy") { //Tema Sketchy
        if (viewTitle.includes("Dashboard"))
        {
            imgNav.src = "/img/sketchy/dashboard_sketchy.png";
        }
        if (viewTitle.includes("Lançamento"))
        {
            imgNav.src = "/img/sketchy/money_sketchy.png";
        }
        if (viewTitle.includes("Categoria"))
        {
            imgNav.src = "/img/sketchy/categorias_sketchy.png";
        }
        if (viewTitle.includes("Extrato"))
        {
            imgNav.src = "/img/sketchy/extrato_sketchy.jpg";
        }
        if (viewTitle.includes("Conta"))
        {
            imgNav.src = "/img/sketchy/cofre_sketchy.png";
        }
        if (viewTitle.includes("Preferências"))
        {
            imgNav.src = "/img/sketchy/configuracoes_sketchy.png";
        }
        if (viewTitle.includes("Temas"))
        {
            imgNav.src = "/img/temas.png";
            imgNav.style.scale = "0.7";
        }
    }
    else if(theme === "Vapor")
    {
        if (viewTitle.includes("Dashboard"))
        {
            imgNav.src = "img/finances.png";
        }
        if (viewTitle.includes("Lançamento"))
        {
            imgNav.src = "/img/lancamentos.png";
        }
        if (viewTitle.includes("Categoria"))
        {
            imgNav.src = "/img/foto-categorias.png"
        }
        if (viewTitle.includes("Extrato"))
        {
            imgNav.src = "img/bank-statement.png"
        }
        if (viewTitle.includes("Conta"))
        {
            imgNav.src = "/img/cofre.png";
        }
        if (viewTitle.includes("Preferências"))
        {
            imgNav.src = "/img/configuracoes_vapor.png";
        }
        if (viewTitle.includes("Temas"))
        {
            imgNav.src = "/img/temas.png";
            imgNav.style.scale = "0.7";
        }
    }
});