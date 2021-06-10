function passUrl(imgUrl) {
    document.getElementById("modalImage").attributes.getNamedItem("src").value = imgUrl;
}

function radioClick(pricea) {
   
    var methodPrice = document.getElementById("method");
    methodPrice.innerHTML =  pricea ;   
    var price = parseFloat(document.getElementById("total").innerHTML);
    var sumTotal = parseFloat(price + pricea);
    document.getElementById("sumTotal").innerHTML = sumTotal;
  
}