setTimeout(()=> {
    $(".alert").fadeOut("slow", () => {
        $(this).alert("close");
    })
}, 3500)