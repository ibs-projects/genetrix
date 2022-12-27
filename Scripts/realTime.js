alert('fff')

function AddLi(userName, message, idItem, right) {
    var li = "<li class=\"" + isRight + "\">"
        + "<div class=\"conversation-list\">"
        + "<div class=\"ctext-wrap\">"
        + "<div class=\"ctext-wrap-content\">"
        + "<h5 class=\"font-size-14 conversation-name\">" + "<a href=\"#\" class=\"text-dark\">" + userName + "</a>" + "<span class=\"d-inline-block font-size-12 text-muted ms-2\">10: 00 </span>" + "</h5>"
            + "<p class=\"mb-0\">"
        +message
        + "</p>"
        + "</div>"
        + "<div class=\"dropdown align-self-start\">"
        + "<a class=\"dropdown-toggle\" href=\"#\" role=\"button\" data-bs-toggle=\"dropdown\" aria-haspopup=\"true\" aria-expanded=\"false\">"
        + "<i class=\"uil uil-ellipsis-v\">" + "</i>"
        + "</a>"
        + "<div class=\"dropdown-menu\">"
        + "<a class=\"dropdown-item\" href=\"#" + idItem + "\">Copy</a>"
        + "<a class=\"dropdown-item\" href=\"#" + idItem + "\">Save</a>"
        + "<a class=\"dropdown-item\" href=\"#" + idItem + "\">Forward</a>"
        + "<a class=\"dropdown-item\" href=\"#" + idItem + "\">Delete</a>"
        + "</div>"
        + "</div>"
        + "</div>"
        + "</div>"
        + "</li>";
    alert('fonction:  ' + li);
    return li;
}