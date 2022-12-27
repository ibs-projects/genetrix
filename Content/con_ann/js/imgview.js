alert("ddd");
$(".images img").click(function () {
    alert("rr");
    $("full-image").attr("src", $(this).attr("src"));
    $('image-viewer').show();
});

$("#image-viewer .close").click(function () {
    $('image-viewer').hide();
});